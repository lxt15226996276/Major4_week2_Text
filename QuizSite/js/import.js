let pendingQuestions = [];
let parsedFromFile = [];

const $ = (sel) => document.querySelector(sel);

function normalizeRow(row, index) {
  const get = (...keys) => {
    for (const k of keys) {
      const v = row[k] ?? row[k?.toLowerCase?.()];
      if (v != null && String(v).trim()) return String(v).trim();
    }
    return "";
  };
  const q = get("question", "题干", "题目", "问题");
  const a = get("answer", "答案", "参考答案");
  if (!q) return null;
  const tagsRaw = get("tags", "标签");
  return {
    question: q,
    answer: a || "（暂无答案）",
    category: get("category", "分类", "单元", "章节"),
    level: get("level", "职级"),
    tags: tagsRaw ? tagsRaw.split(/[|,，]/).map((t) => t.trim()).filter(Boolean) : [],
  };
}

function parseCSV(text) {
  const lines = text.replace(/\r/g, "").split("\n").filter(Boolean);
  if (lines.length < 2) return [];
  const headers = lines[0].split(",").map((h) => h.trim());
  const rows = [];
  for (let i = 1; i < lines.length; i++) {
    const vals = lines[i].split(",");
    const row = {};
    headers.forEach((h, j) => (row[h] = (vals[j] || "").trim()));
    const q = normalizeRow(row, i);
    if (q) rows.push(q);
  }
  return rows;
}

function parseJSON(text) {
  const data = JSON.parse(text);
  const list = Array.isArray(data) ? data : data.questions || [];
  return list.map((row, i) => normalizeRow(row, i)).filter(Boolean);
}

function parseXLSX(arrayBuffer) {
  const wb = XLSX.read(arrayBuffer, { type: "array" });
  const all = [];
  wb.SheetNames.forEach((sheetName) => {
    const sheet = wb.Sheets[sheetName];
    const rows = XLSX.utils.sheet_to_json(sheet, { defval: "" });
    rows.forEach((row, i) => {
      const q = normalizeRow(row, i);
      if (q) {
        if (!q.category) q.category = sheetName;
        all.push(q);
      }
    });
  });
  return all;
}

function showPreview(questions) {
  parsedFromFile = questions;
  pendingQuestions = questions;
  $("#preview-count").textContent = questions.length;
  const list = $("#preview-list");
  list.innerHTML = "";
  questions.slice(0, 5).forEach((q) => {
    const li = document.createElement("li");
    li.textContent = q.question.slice(0, 60) + (q.question.length > 60 ? "…" : "");
    list.appendChild(li);
  });
  if (questions.length > 5) {
    const li = document.createElement("li");
    li.textContent = `… 还有 ${questions.length - 5} 题`;
    list.appendChild(li);
  }
  $("#preview").classList.remove("hidden");
  $("#btn-upload").disabled = questions.length === 0;
  updateManualCount();
}

async function handleFile(file) {
  const ext = file.name.split(".").pop().toLowerCase();
  const buf = await file.arrayBuffer();
  let questions = [];
  if (ext === "csv") {
    questions = parseCSV(new TextDecoder("utf-8").decode(buf));
  } else if (ext === "json") {
    questions = parseJSON(new TextDecoder("utf-8").decode(buf));
  } else if (ext === "xlsx" || ext === "xls") {
    questions = parseXLSX(buf);
  }
  if (!questions.length) {
    setStatus("未能解析到题目，请检查列名是否为「题干/答案」", true);
    return;
  }
  if (!$("#bank-name").value) {
    $("#bank-name").value = file.name.replace(/\.[^.]+$/, "");
  }
  showPreview(questions);
  setStatus(`已解析 ${questions.length} 题，点击「提交到服务器」`);
}

async function submitToServer(questions, name) {
  if (!questions.length) return;
  setStatus("提交中…");
  try {
    const res = await fetch("/api/import", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name: name || "自定义题库", questions }),
    });
    const data = await res.json();
    if (data.ok) {
      setStatus(`✅ ${data.message}。返回刷题页刷新即可。`, false);
      pendingQuestions = [];
      parsedFromFile = [];
      $("#btn-upload").disabled = true;
      $("#btn-submit-manual").disabled = true;
      updateManualCount();
    } else {
      setStatus(data.message || "提交失败", true);
    }
  } catch {
    setStatus(
      "无法连接服务器。请用 python QuizSite/scripts/server.py 启动，或把 CSV/JSON 放到 QuizSite/imports/ 后运行 build_all.py",
      true
    );
  }
}

function setStatus(msg, isError = false) {
  const el = $("#upload-status");
  el.textContent = msg;
  el.className = isError ? "status error" : "status";
}

function updateManualCount() {
  $("#manual-count").textContent = pendingQuestions.length;
  $("#btn-submit-manual").disabled = pendingQuestions.length === 0;
}

async function initImportPage() {
  if (await isStaticSite()) {
    $("#static-import-notice")?.classList.remove("hidden");
    $("#import-intro").textContent = "预览与解析文件可用；提交需本地服务器或重新部署静态站。";
    $("#btn-upload").disabled = true;
    $("#btn-submit-manual").disabled = true;
    setStatus("永久站点：请在本机改题库后运行「部署到公网.bat」", false);
  }
}

function bindEvents() {
  const drop = $("#drop-zone");
  const input = $("#file-input");

  drop.addEventListener("click", () => input.click());
  input.addEventListener("change", () => {
    if (input.files[0]) handleFile(input.files[0]);
  });
  drop.addEventListener("dragover", (e) => {
    e.preventDefault();
    drop.classList.add("dragover");
  });
  drop.addEventListener("dragleave", () => drop.classList.remove("dragover"));
  drop.addEventListener("drop", (e) => {
    e.preventDefault();
    drop.classList.remove("dragover");
    if (e.dataTransfer.files[0]) handleFile(e.dataTransfer.files[0]);
  });

  $("#btn-upload").addEventListener("click", () => {
    submitToServer(parsedFromFile, $("#bank-name").value);
  });

  $("#btn-add-manual").addEventListener("click", () => {
    const q = $("#manual-q").value.trim();
    const a = $("#manual-a").value.trim();
    if (!q) {
      setStatus("请填写题目", true);
      return;
    }
    pendingQuestions.push({
      question: q,
      answer: a || "（暂无答案）",
      category: $("#manual-cat").value.trim(),
      tags: [],
    });
    $("#manual-q").value = "";
    $("#manual-a").value = "";
    updateManualCount();
    setStatus(`已加入，待提交 ${pendingQuestions.length} 题`);
  });

  $("#btn-submit-manual").addEventListener("click", () => {
    submitToServer(pendingQuestions, $("#bank-name").value || "手动录入");
  });
}

bindEvents();
initImportPage();
