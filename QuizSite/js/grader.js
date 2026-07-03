/**
 * 简答题智能判分：关键词匹配 + 文本相似度
 */
const QuizGrader = (() => {
  const STOP = new Set([
    "的", "是", "在", "和", "与", "或", "及", "等", "有", "为", "了", "可以", "能够",
    "一种", "一个", "这种", "通过", "进行", "使用", "包括", "主要", "通常", "一般",
    "the", "a", "an", "is", "are", "to", "of", "and", "or", "for", "with",
  ]);

  function norm(text) {
    return (text || "")
      .toLowerCase()
      .replace(/\s+/g, "")
      .replace(/[，。、；;：:！!？?（）()【】\[\]《》""''\n\r]/g, "");
  }

  function extractKeywords(reference) {
    const raw = reference || "";
    const tokens = new Set();

    const en = raw.match(/[a-zA-Z][a-zA-Z0-9\s_-]{1,}/g) || [];
    en.forEach((w) => {
      const t = w.trim().replace(/\s+/g, " ");
      if (t.length >= 2) tokens.add(t);
      t.split(/[\s/_-]+/).forEach((p) => {
        if (p.length >= 2) tokens.add(p);
      });
    });

    raw
      .split(/[，。、；;|/\\()\[\]（）\n\r]+/)
      .map((s) => s.trim())
      .forEach((seg) => {
        if (seg.length >= 2 && seg.length <= 24 && !STOP.has(seg)) tokens.add(seg);
      });

    raw.match(/[\u4e00-\u9fff]{2,8}/g)?.forEach((w) => {
      if (!STOP.has(w)) tokens.add(w);
    });

    return [...tokens].filter((t) => t.length >= 2).slice(0, 20);
  }

  function similarity(a, b) {
    if (!a || !b) return 0;
    const sa = norm(a);
    const sb = norm(b);
    if (!sa || !sb) return 0;
    if (sa === sb) return 1;
    if (sa.includes(sb) || sb.includes(sa)) return 0.85;

    const bigrams = (s) => {
      const set = new Set();
      for (let i = 0; i < s.length - 1; i++) set.add(s.slice(i, i + 2));
      return set;
    };
    const A = bigrams(sa);
    const B = bigrams(sb);
    if (!A.size || !B.size) return 0;
    let inter = 0;
    A.forEach((x) => {
      if (B.has(x)) inter++;
    });
    return (2 * inter) / (A.size + B.size);
  }

  function grade(userAnswer, reference) {
    const user = (userAnswer || "").trim();
    const ref = (reference || "").trim();

    if (!user) {
      return {
        level: "empty",
        score: 0,
        percent: 0,
        title: "还没写答案",
        message: "下次可以先写几个关键词，再对照参考答案，记忆更深。",
        matched: [],
        missing: extractKeywords(ref).slice(0, 8),
      };
    }

    if (!ref || ref === "（暂无答案）") {
      return {
        level: "partial",
        score: 0.5,
        percent: 50,
        title: "暂无标准答案可比对",
        message: "请对照讲义或教材自行判断。",
        matched: [],
        missing: [],
      };
    }

    const keywords = extractKeywords(ref);
    const userNorm = norm(user);
    const matched = keywords.filter((k) => userNorm.includes(norm(k)));
    const missing = keywords.filter((k) => !userNorm.includes(norm(k)));

    let keywordScore = keywords.length ? matched.length / keywords.length : 0;
    const textSim = similarity(user, ref);
    const score = Math.min(1, keywordScore * 0.65 + textSim * 0.35);

    let level, title, message;
    if (score >= 0.72) {
      level = "correct";
      title = "答得不错！";
      message = `命中 ${matched.length}/${keywords.length} 个要点，继续保持。`;
    } else if (score >= 0.38) {
      level = "partial";
      title = "部分正确";
      message = `对了 ${matched.length} 个要点，还有 ${missing.length} 个可以补充。`;
    } else {
      level = "wrong";
      title = "和参考答案差距较大";
      message = "看看下面缺失的要点，再记一遍。";
    }

    return {
      level,
      score,
      percent: Math.round(score * 100),
      title,
      message,
      matched: matched.slice(0, 12),
      missing: missing.slice(0, 10),
    };
  }

  return { grade, extractKeywords };
})();

window.QuizGrader = QuizGrader;
