const STORAGE_KEY = "quiz-progress-v2";
const DRAFT_KEY = "quiz-drafts-v1";
const LS_TRACK = "quiz-track";
const LS_EXAM_MODE = "quiz-exam-mode";
const LS_EXAM_SEL = "quiz-exam-selection";

const state = {
  manifest: null,
  trackId: localStorage.getItem(LS_TRACK) || "exam",
  examMode: localStorage.getItem(LS_EXAM_MODE) || "week",
  examSelection: localStorage.getItem(LS_EXAM_SEL) || "exam-week-1",
  bankId: null,
  allQuestions: [],
  pool: [],
  current: null,
  lastId: null,
  sessionCount: 0,
  progress: {},
  drafts: {},
  answerVisible: false,
  history: [],
  historyIndex: -1,
  lastGrade: null,
};

const $ = (sel) => document.querySelector(sel);

function examTrack() {
  return state.manifest?.tracks?.find((t) => t.id === "exam");
}

function interviewTrack() {
  return state.manifest?.tracks?.find((t) => t.id === "interview");
}

async function init() {
  loadProgress();
  loadDrafts();
  bindEvents();
  updateSoundToggle();
  await loadManifest();
  await applyTrackUI(false);
  await reloadBank(true);
  await showAccessUrl();
}

function loadProgress() {
  try {
    state.progress = JSON.parse(localStorage.getItem(STORAGE_KEY) || "{}");
  } catch {
    state.progress = {};
  }
}

function loadDrafts() {
  try {
    state.drafts = JSON.parse(sessionStorage.getItem(DRAFT_KEY) || "{}");
  } catch {
    state.drafts = {};
  }
}

function saveDrafts() {
  sessionStorage.setItem(DRAFT_KEY, JSON.stringify(state.drafts));
}

function draftKey(id) {
  return `${state.bankId}:${id}`;
}

function saveCurrentDraft() {
  if (!state.current) return;
  state.drafts[draftKey(state.current.id)] = $("#my-answer").value;
  saveDrafts();
}

function bankProgress() {
  const key = state.bankId || "_";
  if (!state.progress[key]) {
    state.progress[key] = { seen: [], known: [], unknown: [] };
  }
  return state.progress[key];
}

function saveProgress() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state.progress));
}

async function loadManifest() {
  const res = await fetch("data/manifest.json");
  state.manifest = await res.json();
  const exam = examTrack();
  if (exam && !localStorage.getItem(LS_EXAM_SEL)) {
    state.examSelection = exam.defaultSelection || "exam-week-1";
  }
}

function applyTrackUI(save = true) {
  document.body.classList.toggle("mode-exam", state.trackId === "exam");
  document.body.classList.toggle("mode-interview", state.trackId === "interview");

  $("#filter-track").value = state.trackId;
  $("#filter-exam-mode").value = state.examMode;

  if (state.trackId === "exam") {
    populateExamSelection();
  }

  if (save) {
    localStorage.setItem(LS_TRACK, state.trackId);
    localStorage.setItem(LS_EXAM_MODE, state.examMode);
    localStorage.setItem(LS_EXAM_SEL, state.examSelection);
  }
}

function populateExamSelection() {
  const exam = examTrack();
  if (!exam) return;

  const modeData = exam.modes[state.examMode];
  const sel = $("#filter-exam-selection");
  sel.innerHTML = "";

  (modeData?.options || []).forEach((opt) => {
    const o = document.createElement("option");
    o.value = opt.id;
    if (state.examMode === "week") {
      o.textContent = opt.sheetName ? `${opt.sheetName} · ${opt.count} 题` : `${opt.name}`;
    } else {
      o.textContent = `${opt.unitName || opt.name} · ${opt.count} 题`;
    }
    sel.appendChild(o);
  });

  if (!modeData?.options?.find((o) => o.id === state.examSelection)) {
    state.examSelection = modeData?.options?.[0]?.id;
  }
  sel.value = state.examSelection;

  $("#exam-selection-label").textContent = state.examMode === "week" ? "周次（Excel 工作表）" : "单元（Excel 对应单元）";
  const current = modeData?.options?.find((o) => o.id === state.examSelection);
  $("#exam-hint").textContent =
    state.examMode === "week"
      ? current?.sheetName
        ? `${current.sheetName}，共 ${current.count} 题`
        : modeData?.hint || ""
      : current?.unitName
        ? `${current.unitName}，共 ${current.count} 题`
        : modeData?.hint || "";

  const track = examTrack();
  if (track) {
    $("#bank-subtitle").textContent =
      state.examMode === "week"
        ? `考试及格 · ${current?.sheetName || ""}`
        : `考试及格 · ${current?.unitName || ""}`;
  }
}

async function resolveBankFile() {
  if (state.trackId === "interview") {
    const t = interviewTrack();
    return { file: t.file, bankId: t.bankId, name: t.name };
  }
  const exam = examTrack();
  const opt = exam?.modes?.[state.examMode]?.options?.find((o) => o.id === state.examSelection);
  if (!opt) return null;
  return { file: opt.file, bankId: opt.id, name: opt.name };
}

async function reloadBank(resetNav = true) {
  saveCurrentDraft();
  const resolved = await resolveBankFile();
  if (!resolved) return;

  state.bankId = resolved.bankId;
  $("#bank-subtitle").textContent =
    state.trackId === "interview"
      ? interviewTrack().subtitle
      : $("#bank-subtitle").textContent;

  const res = await fetch(`data/${resolved.file}`);
  const data = await res.json();
  state.allQuestions = data.questions || [];

  if (resetNav) {
    state.history = [];
    state.historyIndex = -1;
    state.sessionCount = 0;
    $("#stat-session").textContent = "0";
  }

  if (state.trackId === "interview") {
    $("#bank-subtitle").textContent = interviewTrack().name;
    populateInterviewFilters();
  } else {
    populateExamSelection();
    const exam = examTrack();
    const opt = exam?.modes?.[state.examMode]?.options?.find((o) => o.id === state.examSelection);
    $("#bank-subtitle").textContent = `考试及格 · ${opt?.unitName || opt?.sheetName || opt?.name || resolved.name}`;
  }

  updateStats();
  refreshPool();

  if (state.allQuestions.length === 0) {
    $("#q-text").textContent = "当前分类下暂无题目，请换周次/单元或补充 Excel 题库";
    $("#question-card").classList.remove("hidden");
    return;
  }

  goToRandomQuestion();
}

function populateInterviewFilters() {
  const domainSel = $("#filter-domain");
  domainSel.innerHTML = '<option value="">全部域</option>';
  [...new Set(state.allQuestions.map((q) => q.domain).filter(Boolean))]
    .sort((a, b) => a - b)
    .forEach((d) => {
      const q = state.allQuestions.find((x) => x.domain === d);
      const opt = document.createElement("option");
      opt.value = String(d);
      opt.textContent = `域${d} · ${q?.domainName || d}`;
      domainSel.appendChild(opt);
    });

  const examSel = $("#filter-exam-ref");
  examSel.innerHTML = '<option value="">不限</option>';
  const exams = new Set();
  state.allQuestions.forEach((q) => {
    if (!q.examRef) return;
    const m = q.examRef.match(/Exam\d+/g);
    if (m) m.forEach((e) => exams.add(e));
  });
  [...exams].sort().forEach((e) => {
    const opt = document.createElement("option");
    opt.value = e;
    opt.textContent = e;
    examSel.appendChild(opt);
  });
}

function getFilters() {
  if (state.trackId === "exam") {
    return { preferUnseen: $("#filter-unseen").checked };
  }
  return {
    domain: $("#filter-domain").value,
    level: $("#filter-level").value,
    exam: $("#filter-exam-ref").value,
    hotOnly: $("#filter-hot").checked,
    preferUnseen: $("#filter-unseen").checked,
  };
}

function matchesFilter(q, f) {
  if (state.trackId === "exam") return true;
  if (f.domain && String(q.domain) !== f.domain) return false;
  if (f.level && q.level !== f.level) return false;
  if (f.exam && (!q.examRef || !q.examRef.includes(f.exam))) return false;
  if (f.hotOnly && !(q.tags || []).includes("高频")) return false;
  return true;
}

function refreshPool() {
  const f = getFilters();
  const prog = bankProgress();
  state.pool = state.allQuestions.filter((q) => matchesFilter(q, f));

  if (f.preferUnseen && state.pool.length > 0) {
    const unseen = state.pool.filter((q) => !prog.seen.includes(q.id));
    if (unseen.length > 0) state.pool = unseen;
  }

  $("#pool-count").textContent = state.pool.length;

  if (state.pool.length === 0) {
    $("#question-card").classList.add("hidden");
    $("#empty-state").classList.remove("hidden");
  } else {
    $("#question-card").classList.remove("hidden");
    $("#empty-state").classList.add("hidden");
  }
}

function findQuestion(id) {
  return state.allQuestions.find((q) => q.id === id);
}

function pushHistory(id) {
  if (state.historyIndex < state.history.length - 1) {
    state.history = state.history.slice(0, state.historyIndex + 1);
  }
  if (state.history[state.historyIndex] !== id) {
    state.history.push(id);
    state.historyIndex = state.history.length - 1;
  }
  updateNavButtons();
}

function showQuestion(q, { addHistory = true } = {}) {
  if (!q) return;
  state.current = q;
  state.lastId = q.id;
  state.lastGrade = null;
  if (addHistory) pushHistory(q.id);

  const prog = bankProgress();
  if (!prog.seen.includes(q.id)) {
    prog.seen.push(q.id);
    saveProgress();
  }

  renderQuestion();
  updateStats();
}

function goToRandomQuestion() {
  refreshPool();
  if (state.pool.length === 0) return;

  let candidates = state.pool;
  if (state.pool.length > 1 && state.lastId) {
    const filtered = state.pool.filter((q) => q.id !== state.lastId);
    if (filtered.length > 0) candidates = filtered;
  }

  showQuestion(candidates[Math.floor(Math.random() * candidates.length)]);
}

function goToPreviousQuestion() {
  if (state.historyIndex <= 0) return;
  saveCurrentDraft();
  QuizSounds.prev();
  state.historyIndex--;
  const q = findQuestion(state.history[state.historyIndex]);
  if (q) showQuestion(q, { addHistory: false });
  updateNavButtons();
}

function updateNavButtons() {
  const canPrev = state.historyIndex > 0;
  $("#btn-prev").disabled = !canPrev;
  const pos = state.history.length ? state.historyIndex + 1 : 0;
  $("#nav-pos").textContent = state.history.length ? `${pos} / ${state.history.length}` : "";
}

function renderQuestion() {
  const q = state.current;
  if (!q) return;

  const domainEl = $("#q-domain");
  if (state.trackId === "interview" && q.domainName) {
    domainEl.textContent = q.domain ? `域${q.domain} · ${q.domainName}` : q.domainName;
    domainEl.classList.remove("hidden");
  } else if (q.unitName || q.category) {
    domainEl.textContent = q.unitName || q.category;
    domainEl.classList.remove("hidden");
  } else {
    domainEl.classList.add("hidden");
  }

  $("#q-level").textContent = q.level || "理论";
  $("#q-id").textContent = q.id;

  const focusEl = $("#q-focus");
  if (q.focus) {
    focusEl.textContent = q.focus;
    focusEl.classList.remove("hidden");
  } else if (q.weekName) {
    focusEl.textContent = q.weekName;
    focusEl.classList.remove("hidden");
  } else {
    focusEl.classList.add("hidden");
  }

  $("#q-text").textContent = q.question;
  $("#q-answer").textContent = q.answer;

  const examEl = $("#q-exam");
  examEl.textContent = q.examRef ? `关联练习：${q.examRef}` : "";

  const tagsEl = $("#q-tags");
  tagsEl.innerHTML = "";
  (q.tags || []).forEach((t) => {
    const span = document.createElement("span");
    span.className = "tag";
    span.textContent = t;
    tagsEl.appendChild(span);
  });

  const followupSec = $("#followup-section");
  if (q.followUp) {
    followupSec.classList.remove("hidden");
    $("#q-followup").textContent = q.followUp;
  } else {
    followupSec.classList.add("hidden");
  }

  $("#my-answer").value = state.drafts[draftKey(q.id)] || "";
  hideFeedback();
  hideAnswer();

  const card = $("#question-card");
  card.style.animation = "none";
  void card.offsetWidth;
  card.style.animation = "";
  updateNavButtons();
}

function renderFeedback(result) {
  const panel = $("#feedback-panel");
  panel.classList.remove("hidden", "fb-correct", "fb-partial", "fb-wrong", "fb-empty");
  panel.classList.add(`fb-${result.level}`);
  $("#fb-title").textContent = result.title;
  $("#fb-message").textContent = result.message;
  $("#fb-score").textContent = result.level === "empty" ? "—" : `${result.percent}%`;

  const matchedEl = $("#fb-matched");
  const missingEl = $("#fb-missing");
  matchedEl.innerHTML = "";
  missingEl.innerHTML = "";

  if (result.matched.length) {
    $("#fb-matched-wrap").classList.remove("hidden");
    result.matched.forEach((w) => {
      const s = document.createElement("span");
      s.className = "fb-tag fb-tag-ok";
      s.textContent = w;
      matchedEl.appendChild(s);
    });
  } else {
    $("#fb-matched-wrap").classList.add("hidden");
  }

  if (result.missing.length) {
    $("#fb-missing-wrap").classList.remove("hidden");
    result.missing.forEach((w) => {
      const s = document.createElement("span");
      s.className = "fb-tag fb-tag-miss";
      missingEl.appendChild(s);
      s.textContent = w;
    });
  } else {
    $("#fb-missing-wrap").classList.add("hidden");
  }
}

function hideFeedback() {
  $("#feedback-panel").classList.add("hidden");
}

function playGradeSound(level) {
  switch (level) {
    case "correct":
      QuizSounds.correct();
      break;
    case "partial":
      QuizSounds.partial();
      break;
    case "wrong":
      QuizSounds.wrong();
      break;
    default:
      QuizSounds.empty();
  }
}

function showAnswer({ silent = false } = {}) {
  QuizSounds.unlock();
  saveCurrentDraft();

  const mine = $("#my-answer").value.trim();

  if (mine) {
    const result = QuizGrader.grade(mine, state.current?.answer);
    state.lastGrade = result;
    renderFeedback(result);
    $("#my-answer-display").textContent = mine;
    $("#my-answer-review").classList.remove("hidden");
    $("#answer-mode-hint").classList.add("hidden");
    if (!silent) {
      QuizSounds.reveal();
      playGradeSound(result.level);
    }
  } else {
    state.lastGrade = null;
    hideFeedback();
    $("#my-answer-review").classList.add("hidden");
    $("#answer-mode-hint").classList.remove("hidden");
    if (!silent) QuizSounds.reveal();
  }

  state.answerVisible = true;
  state.drafts[draftKey(state.current.id) + ":revealed"] = true;
  saveDrafts();

  $("#answer-section").classList.remove("hidden");
  $("#rate-actions").classList.remove("hidden");
  $("#btn-reveal").textContent = "隐藏答案";
  $("#btn-reveal").classList.add("is-active");
}

function hideAnswer() {
  state.answerVisible = false;
  if (state.current) {
    delete state.drafts[draftKey(state.current.id) + ":revealed"];
    saveDrafts();
  }
  hideFeedback();
  $("#answer-mode-hint").classList.add("hidden");
  $("#answer-section").classList.add("hidden");
  $("#rate-actions").classList.add("hidden");
  $("#btn-reveal").textContent = "显示答案";
  $("#btn-reveal").classList.remove("is-active");
}

function toggleAnswer() {
  QuizSounds.click();
  state.answerVisible ? hideAnswer() : showAnswer();
}

function markKnown(isKnown) {
  const id = state.current?.id;
  if (!id) return;
  QuizSounds.click();
  if (isKnown) QuizSounds.correct();
  else QuizSounds.wrong();

  const prog = bankProgress();
  prog.known = prog.known.filter((x) => x !== id);
  prog.unknown = prog.unknown.filter((x) => x !== id);
  if (isKnown) prog.known.push(id);
  else prog.unknown.push(id);
  saveProgress();
  updateStats();
  nextQuestion();
}

function updateStats() {
  const total = state.allQuestions.length;
  const prog = bankProgress();
  const done = new Set([...prog.known, ...prog.unknown]).size;
  const pct = total ? Math.round((prog.seen.length / total) * 100) : 0;
  $("#stat-total").textContent = total;
  $("#stat-done").textContent = done;
  $("#progress-fill").style.width = `${pct}%`;
  $("#progress-pct").textContent = pct;
}

function resetProgress() {
  if (!confirm("确定重置当前分类下的刷题进度？")) return;
  state.progress[state.bankId] = { seen: [], known: [], unknown: [] };
  state.sessionCount = 0;
  saveProgress();
  updateStats();
  $("#stat-session").textContent = "0";
}

function updateSoundToggle() {
  $("#sound-toggle").checked = QuizSounds.isEnabled();
}

async function showAccessUrl() {
  const url = await getShareUrl();
  if (!url) return;
  const site = await fetchSiteMeta();
  const label = $("#access-label");
  if (site?.mode === "static") {
    label.textContent = "永久站点（可收藏，链接不变）：";
  }
  $("#access-url").textContent = url;
  $("#access-banner").classList.remove("hidden");
}

async function onTrackChange() {
  state.trackId = $("#filter-track").value;
  applyTrackUI();
  await reloadBank(true);
}

async function onExamModeChange() {
  state.examMode = $("#filter-exam-mode").value;
  const exam = examTrack();
  const opts = exam?.modes?.[state.examMode]?.options || [];
  state.examSelection = opts.find((o) => o.count > 0)?.id || opts[0]?.id;
  applyTrackUI();
  await reloadBank(true);
}

async function onExamSelectionChange() {
  state.examSelection = $("#filter-exam-selection").value;
  applyTrackUI();
  await reloadBank(true);
}

function bindEvents() {
  $("#filter-track").addEventListener("change", onTrackChange);
  $("#filter-exam-mode").addEventListener("change", onExamModeChange);
  $("#filter-exam-selection").addEventListener("change", onExamSelectionChange);

  $("#btn-prev").addEventListener("click", goToPreviousQuestion);
  $("#btn-next").addEventListener("click", nextQuestion);
  $("#btn-reveal").addEventListener("click", toggleAnswer);
  $("#btn-know").addEventListener("click", () => markKnown(true));
  $("#btn-unknown").addEventListener("click", () => markKnown(false));
  $("#btn-reset-progress").addEventListener("click", resetProgress);

  $("#sound-toggle").addEventListener("change", (e) => {
    QuizSounds.setEnabled(e.target.checked);
    if (e.target.checked) {
      QuizSounds.unlock();
      QuizSounds.click();
    }
  });

  document.body.addEventListener("click", () => QuizSounds.unlock(), { once: true });

  ["filter-domain", "filter-level", "filter-exam-ref", "filter-hot", "filter-unseen"].forEach((id) => {
    $(`#${id}`)?.addEventListener("change", () => {
      refreshPool();
      if (state.current && !state.pool.find((q) => q.id === state.current.id)) goToRandomQuestion();
    });
  });

  document.addEventListener("keydown", (e) => {
    if (["SELECT", "INPUT", "TEXTAREA"].includes(e.target.tagName)) return;
    switch (e.code) {
      case "Space":
        e.preventDefault();
        nextQuestion();
        break;
      case "ArrowLeft":
        goToPreviousQuestion();
        break;
      case "KeyA":
        toggleAnswer();
        break;
      case "Digit1":
        state.answerVisible ? markKnown(true) : showAnswer();
        break;
      case "Digit2":
        if (state.answerVisible) markKnown(false);
        break;
    }
  });
}

function nextQuestion() {
  saveCurrentDraft();
  QuizSounds.next();
  state.sessionCount++;
  $("#stat-session").textContent = state.sessionCount;
  goToRandomQuestion();
}

init();
