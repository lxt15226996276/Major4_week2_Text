import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(__dirname, '..');
const testsRoot = path.resolve(root, '..');

function parseInterviewQuestions(markdown) {
  const questions = [];
  const domainTitles = {};
  const domainRe = /^## (?:[三四五六七八九十]+|[一二]+)、域 (\d+) · (.+)$/gm;
  let m;
  while ((m = domainRe.exec(markdown)) !== null) {
    domainTitles[m[1]] = m[2].trim();
  }

  const lines = markdown.split('\n');
  for (const line of lines) {
    if (!line.startsWith('| INT-')) continue;
    const cols = line.split('|').map((c) => c.trim()).filter(Boolean);
    if (cols.length < 6) continue;
    const [id, freq, level, question, focus, answer, exam = '—'] = cols;
    const domain = id.match(/^INT-(\d+)-/)[1];
    questions.push({
      id,
      source: 'interview',
      domain: Number(domain),
      domainName: domainTitles[domain] || `域${domain}`,
      frequency: freq === '★' ? 'high' : 'normal',
      level: level.replace(/\s/g, ''),
      question,
      focus,
      answer,
      exam: exam.replace(/^↔\s*/, '').trim(),
      tags: [domainTitles[domain] || `域${domain}`, level.replace(/\s/g, '')].filter(Boolean),
    });
  }
  return questions;
}

function parseXiaozaoFile(filePath, meta) {
  const text = fs.readFileSync(filePath, 'utf8');
  const questions = [];

  const qSection = text.match(/## (?:十|十一)、自检题[\s\S]*?(?=\n---|\n## )/);
  if (!qSection) return questions;

  const qLines = qSection[0].match(/^\d+\.\s+(.+)$/gm) || [];
  const qTexts = qLines.map((line) => line.replace(/^\d+\.\s+/, '').trim());

  const aSection =
    text.match(/## (?:§11-A|十一-A|十-A)[^\n]*[\s\S]*?(?=\n---|\n## (?!§|\d))/i) ||
    text.match(/## (?:§11-A|十一-A|十-A)[^\n]*[\s\S]*$/i);
  const answerBlocks = [];
  if (aSection) {
    const blocks = aSection[0].split(/\n(?=\d+\.\s+\*\*)/);
    for (const block of blocks) {
      const numMatch = block.match(/^(\d+)\.\s+\*\*(.+?)\*\*/);
      if (numMatch) {
        const body = block
          .replace(/^\d+\.\s+\*\*.+?\*\*\s*/s, '')
          .replace(/^→\s*/, '')
          .trim();
        answerBlocks.push({ num: Number(numMatch[1]), title: numMatch[2], body });
      }
    }
  }

  qTexts.forEach((q, i) => {
    const ans = answerBlocks.find((a) => a.num === i + 1);
    questions.push({
      id: `XZ-${meta.id}-${String(i + 1).padStart(2, '0')}`,
      source: 'xiaozao',
      domain: meta.domain,
      domainName: meta.domainName,
      frequency: 'normal',
      level: '小灶',
      question: q.replace(/`/g, ''),
      focus: meta.module,
      answer: ans ? (ans.title ? `${ans.title}\n\n${ans.body}` : ans.body) : '（暂无标准答案，请对照讲义）',
      exam: meta.exam,
      tags: ['小灶课堂', meta.module],
    });
  });

  return questions;
}

const interviewPath = path.join(testsRoot, '阶段性主程成长路线', '大厂面试题_域映射.md');
const interviewMd = fs.readFileSync(interviewPath, 'utf8');
const interviewQuestions = parseInterviewQuestions(interviewMd);

const xiaozaoDir = path.join(testsRoot, '阶段性主程成长路线', '小灶课堂');
const xiaozaoMeta = [
  { file: '小灶01_Exam05血量商业四层.md', id: '01', domain: 6, domainName: '架构与工程化', module: '血量系统', exam: 'Exam05' },
  { file: '小灶02_Exam06Loading商业分层.md', id: '02', domain: 2, domainName: 'Unity核心机制', module: 'Loading', exam: 'Exam06' },
  { file: '小灶03_Exam07Dictionary商业四层.md', id: '03', domain: 1, domainName: 'C#与OOP', module: 'Dictionary登录', exam: 'Exam07' },
];

const xiaozaoQuestions = xiaozaoMeta.flatMap((meta) =>
  parseXiaozaoFile(path.join(xiaozaoDir, meta.file), meta)
);

const all = [...interviewQuestions, ...xiaozaoQuestions];
const output = {
  version: 1,
  generatedAt: new Date().toISOString(),
  total: all.length,
  domains: [...new Set(all.map((q) => q.domain))].sort((a, b) => a - b),
  questions: all,
};

fs.mkdirSync(path.join(root, 'data'), { recursive: true });
fs.writeFileSync(path.join(root, 'data', 'questions.json'), JSON.stringify(output, null, 2), 'utf8');

console.log(`✅ 已生成 ${all.length} 道题`);
console.log(`   面试题: ${interviewQuestions.length}`);
console.log(`   小灶题: ${xiaozaoQuestions.length}`);
