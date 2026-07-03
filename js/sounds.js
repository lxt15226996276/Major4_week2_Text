/**
 * Web Audio 音效 — 无需外部音频文件
 */
const QuizSounds = (() => {
  let ctx = null;
  let enabled = localStorage.getItem("quiz-sound") !== "off";

  function ensureCtx() {
    if (!ctx) ctx = new (window.AudioContext || window.webkitAudioContext)();
    if (ctx.state === "suspended") ctx.resume();
    return ctx;
  }

  function tone(freq, duration, type = "sine", gain = 0.12, when = 0) {
    if (!enabled) return;
    try {
      const c = ensureCtx();
      const osc = c.createOscillator();
      const g = c.createGain();
      osc.type = type;
      osc.frequency.value = freq;
      g.gain.setValueAtTime(gain, c.currentTime + when);
      g.gain.exponentialRampToValueAtTime(0.001, c.currentTime + when + duration);
      osc.connect(g);
      g.connect(c.destination);
      osc.start(c.currentTime + when);
      osc.stop(c.currentTime + when + duration + 0.05);
    } catch {
      /* 静默失败 */
    }
  }

  function chord(notes, duration, gain = 0.08) {
    notes.forEach((f, i) => tone(f, duration, "sine", gain, i * 0.04));
  }

  return {
    isEnabled: () => enabled,
    setEnabled(v) {
      enabled = v;
      localStorage.setItem("quiz-sound", v ? "on" : "off");
      if (v) ensureCtx();
    },
    unlock() {
      ensureCtx();
    },
    click() {
      tone(520, 0.06, "triangle", 0.06);
    },
    next() {
      tone(640, 0.08, "triangle", 0.07);
    },
    prev() {
      tone(480, 0.08, "triangle", 0.07);
    },
    reveal() {
      tone(740, 0.1, "sine", 0.08);
      tone(880, 0.12, "sine", 0.06, 0.08);
    },
    correct() {
      chord([523, 659, 784], 0.35, 0.1);
    },
    partial() {
      tone(440, 0.15, "triangle", 0.09);
      tone(520, 0.2, "triangle", 0.07, 0.12);
    },
    wrong() {
      tone(220, 0.25, "sawtooth", 0.06);
      tone(180, 0.3, "sawtooth", 0.05, 0.15);
    },
    empty() {
      tone(380, 0.12, "sine", 0.05);
    },
  };
})();

window.QuizSounds = QuizSounds;
