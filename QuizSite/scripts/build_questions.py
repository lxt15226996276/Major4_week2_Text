#!/usr/bin/env python3
"""从 大厂面试题_域映射.md 生成 QuizSite/data/questions.json"""

import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
MD_PATH = ROOT / "Tests" / "阶段性主程成长路线" / "大厂面试题_域映射.md"
OUT_PATH = Path(__file__).resolve().parents[1] / "data" / "questions.json"

DOMAIN_NAMES = {
    1: "C# 与面向对象进阶",
    2: "Unity 核心机制深入",
    3: "渲染与图形",
    4: "物理与动画",
    5: "性能优化",
    6: "架构与工程化",
    7: "网络与多人",
    8: "编辑器扩展与工具链",
    9: "底层与引擎原理",
    10: "跨领域能力",
}

LEVEL_MAP = {"中": "中级", "高": "高级", "主程": "主程"}


def parse_domains(text: str) -> dict[int, str]:
    domains = {}
    for m in re.finditer(r"^## [三四五六七八九十]+、域 (\d+) · (.+)$", text, re.M):
        domains[int(m.group(1))] = m.group(2).strip()
    return domains


def parse_followups(text: str) -> dict[str, str]:
    followups = {}
    for m in re.finditer(
        r"\*\*(?:中级|高级)追问答\*\*：\s*(INT-\d+-\d+)\s*→\s*「(.+?)」",
        text,
    ):
        followups[m.group(1)] = m.group(2)
    return followups


def parse_questions(text: str, domains: dict[int, str], followups: dict[str, str]) -> list:
    questions = []
    for line in text.splitlines():
        if not line.startswith("| INT-"):
            continue
        cols = [c.strip() for c in line.strip("|").split("|")]
        if len(cols) < 7:
            continue

        qid = cols[0]
        m = re.match(r"INT-(\d+)-(\d+)", qid)
        if not m:
            continue

        domain_num = int(m.group(1))
        freq = cols[1]
        level_raw = cols[2]
        level = LEVEL_MAP.get(level_raw, level_raw)

        tags = []
        if "★" in freq:
            tags.append("高频")
        if "🔧" in freq:
            tags.append("需演示")
        if "📋" in freq:
            tags.append("偏架构")

        questions.append(
            {
                "id": qid,
                "domain": domain_num,
                "domainName": domains.get(domain_num, DOMAIN_NAMES.get(domain_num, f"域{domain_num}")),
                "level": level,
                "question": cols[3],
                "focus": cols[4],
                "answer": cols[5],
                "examRef": cols[6] if cols[6] not in ("—", "") else None,
                "tags": tags,
                "followUp": followups.get(qid),
            }
        )
    return questions


def main():
    text = MD_PATH.read_text(encoding="utf-8")
    domains = parse_domains(text)
    followups = parse_followups(text)
    questions = parse_questions(text, domains, followups)

    OUT_PATH.parent.mkdir(parents=True, exist_ok=True)
    payload = {
        "title": "Unity 大厂面试随机刷题",
        "source": "Tests/阶段性主程成长路线/大厂面试题_域映射.md",
        "version": "1.0",
        "total": len(questions),
        "domains": [
            {"id": i, "name": DOMAIN_NAMES[i]} for i in sorted(DOMAIN_NAMES)
        ],
        "questions": questions,
    }
    OUT_PATH.write_text(json.dumps(payload, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Generated {len(questions)} questions -> {OUT_PATH}")


if __name__ == "__main__":
    main()
