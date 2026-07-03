const STORAGE_KEY = 'quizweb-progress-v1';

const state = {
  allQuestions: [],
  filtered: [],
  current: null,
  mode: 'random',
  seqIndex: 0,
  filters: { source: 'all', domain: 'all', level: 'all', exam: 'all' },
  progress: loadProgress(),
};

function loadProgress() {
  try {
    return JSON.parse(localStorage.getItem(STORAGE_KEY)) || { today: '', count: 0, mastered: [], seen: [] };
  } catch {
    return { today: '', count: 0, mastered: [], seen: [] };
  }
}

function saveProgress() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state.progress));
}

function todayKey() {
  return new Date().toISOString().slice(0, 10);
}

function bumpToday() {
  const t = todayKey();
  if (state.progress.today !== t) {
    state.progress.today = t;
    state.progress.count = 0;
  }
  state.progress.count += 1;
  saveProgress();
}

function shuffle(arr) {
  const a = [...arr];
  for (let i = a.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [a[i], a[j]] = [a[j], a[i]];
  }
  return a;
}

function applyFilters() {
  const { source, domain, level, exam } = state.filters;
  state.filtered = state.allQuestions.filter((q) => {
    if (source !== 'all' && q.source !== source) return false;
    if (domain !== 'all' && q.domain !== Number(domain)) return false;
    if (level !== 'all' && q.level !== level) return false;
    if (exam !== 'all') {
      const e = q.exam.toLowerCase();
      if (exam === 'none') return e === '—' || e === '专题' || e.startsWith('域');
      if (!e.toLowerCase().includes(exam.toLowerCase())) return false;
    }
    return true;
  });
  state.seqIndex = 0;
  updateStats();
}

function pickNext() {
  if (state.filtered.length === 0) {
    state.current = null;
    return;
  }
  if (state.mode === 'sequential') {
    state.current = state.filtered[state.seqIndex % state.filtered.length];
    state.seqIndex += 1;
  } else {
    state.current = state.filtered[Math.floor(Math.random() * state.filtered.length)];
  }
  if (state.current && !state.progress.seen.includes(state.current.id)) {
    state.progress.seen.push(state.current.id);
    saveProgress();
  }
  bumpToday();
  renderQuestion();
}

function renderQuestion() {
  const card = document.getElementById('questionCard');
  const q = state.current;

  if (!q) {
    document.getElementById('qText').textContent = '当前筛选下没有题目，请调整左侧筛选条件。';
    document.getElementById('qFocus').textContent = '';
    document.getElementById('qId').textContent = '—';
    document.getElementById('qDomain').textContent = '—';
    document.getElementById('qLevel').textContent = '—';
    document.getElementById('qFreq').classList.add('hidden');
    document.getElementById('answerPanel').classList.add('hidden');
    document.getElementById('btnReveal').classList.add('hidden');
    document.getElementById('btnMastered').classList.add('hidden');
    document.getElementById('progressFill').style.width = '0%';
    document.getElementById('progressText').textContent = '';
    return;
  }

  document.getElementById('btnReveal').classList.remove('hidden');
  document.getElementById('qId').textContent = q.id;
  document.getElementById('qDomain').textContent = `域${q.domain} · ${q.domainName}`;
  document.getElementById('qLevel').textContent = q.level;
  document.getElementById('qFreq').classList.toggle('hidden', q.frequency !== 'high');
  document.getElementById('qText').textContent = q.question;
  document.getElementById('qFocus').textContent = [q.focus, q.exam !== '—' ? `关联：${q.exam}` : ''].filter(Boolean).join(' · ');
  document.getElementById('qAnswer').textContent = q.answer;

  const answerPanel = document.getElementById('answerPanel');
  answerPanel.classList.add('hidden');
  document.getElementById('btnReveal').textContent = '显示答案';
  document.getElementById('btnReveal').onclick = () => {
    answerPanel.classList.remove('hidden');
    document.getElementById('btnReveal').classList.add('hidden');
    document.getElementById('btnMastered').classList.remove('hidden');
  };

  const mastered = state.progress.mastered.includes(q.id);
  const btnM = document.getElementById('btnMastered');
  btnM.textContent = mastered ? '✓ 已掌握' : '标记已掌握';
  btnM.classList.toggle('hidden', true);
  btnM.onclick = () => {
    if (!state.progress.mastered.includes(q.id)) {
      state.progress.mastered.push(q.id);
    }
    btnM.textContent = '✓ 已掌握';
    saveProgress();
    updateStats();
  };

  const seenInPool = state.filtered.filter((x) => state.progress.seen.includes(x.id)).length;
  const pct = state.filtered.length ? Math.round((seenInPool / state.filtered.length) * 100) : 0;
  document.getElementById('progressFill').style.width = `${pct}%`;
  document.getElementById('progressText').textContent = `本轮已见 ${seenInPool} / ${state.filtered.length} 题（${pct}%）`;
}

function updateStats() {
  document.getElementById('statToday').textContent = state.progress.count;
  document.getElementById('statPool').textContent = state.filtered.length;
  document.getElementById('statMastered').textContent = state.progress.mastered.length;
}

function populateFilters(data) {
  const domainSelect = document.getElementById('domainFilter');
  const domains = [...new Map(data.questions.map((q) => [q.domain, q.domainName])).entries()].sort((a, b) => a[0] - b[0]);
  domains.forEach(([num, name]) => {
    const opt = document.createElement('option');
    opt.value = num;
    opt.textContent = `域${num} · ${name}`;
    domainSelect.appendChild(opt);
  });

  const examSelect = document.getElementById('examFilter');
  const exams = new Set();
  data.questions.forEach((q) => {
    if (q.exam && q.exam !== '—' && !q.exam.startsWith('域') && q.exam !== '专题') {
      q.exam.split(/[/\s]/).forEach((part) => {
        const m = part.match(/Exam\d+/i);
        if (m) exams.add(m[0]);
      });
    }
  });
  [...exams].sort().forEach((ex) => {
    const opt = document.createElement('option');
    opt.value = ex;
    opt.textContent = ex;
    examSelect.appendChild(opt);
  });
  const noneOpt = document.createElement('option');
  noneOpt.value = 'none';
  noneOpt.textContent = '无关联练习';
  examSelect.appendChild(noneOpt);
}

function bindUI() {
  document.querySelectorAll('#sourceFilter .chip').forEach((btn) => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('#sourceFilter .chip').forEach((b) => b.classList.remove('active'));
      btn.classList.add('active');
      state.filters.source = btn.dataset.source;
      applyFilters();
      pickNext();
    });
  });

  document.querySelectorAll('#levelFilter .chip').forEach((btn) => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('#levelFilter .chip').forEach((b) => b.classList.remove('active'));
      btn.classList.add('active');
      state.filters.level = btn.dataset.level;
      applyFilters();
      pickNext();
    });
  });

  document.getElementById('domainFilter').addEventListener('change', (e) => {
    state.filters.domain = e.target.value;
    applyFilters();
    pickNext();
  });

  document.getElementById('examFilter').addEventListener('change', (e) => {
    state.filters.exam = e.target.value;
    applyFilters();
    pickNext();
  });

  document.querySelectorAll('.mode-btn').forEach((btn) => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.mode-btn').forEach((b) => b.classList.remove('active'));
      btn.classList.add('active');
      state.mode = btn.dataset.mode;
      state.seqIndex = 0;
      pickNext();
    });
  });

  document.getElementById('btnNext').addEventListener('click', pickNext);

  document.getElementById('btnResetProgress').addEventListener('click', () => {
    if (confirm('确定清除所有本地刷题进度？')) {
      state.progress = { today: todayKey(), count: 0, mastered: [], seen: [] };
      saveProgress();
      updateStats();
      renderQuestion();
    }
  });

  document.addEventListener('keydown', (e) => {
    if (e.target.tagName === 'INPUT' || e.target.tagName === 'SELECT') return;
    if (e.key === ' ' || e.key === 'Enter') {
      e.preventDefault();
      const reveal = document.getElementById('btnReveal');
      if (!reveal.classList.contains('hidden')) reveal.click();
      else pickNext();
    }
    if (e.key === 'n' || e.key === 'N') pickNext();
    if (e.key === 'm' || e.key === 'M') {
      const btn = document.getElementById('btnMastered');
      if (!btn.classList.contains('hidden')) btn.click();
    }
  });
}

async function init() {
  try {
    const res = await fetch('data/questions.json');
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const data = await res.json();
    state.allQuestions = data.questions;
    populateFilters(data);
    bindUI();
    applyFilters();
    pickNext();
  } catch (err) {
    document.getElementById('qText').textContent = `加载题库失败：${err.message}`;
    document.getElementById('qFocus').textContent = '请通过本地服务器打开（见下方说明），不要直接双击 HTML 文件。';
  }
}

init();
