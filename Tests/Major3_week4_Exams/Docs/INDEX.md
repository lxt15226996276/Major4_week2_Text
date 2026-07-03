Unity 考试题库 - 十套试题目录索引
=====================================

本项目将 10 套 Unity 技能考试题整合在同一工程中，每套题完全独立，互不干扰。

目录结构
--------
Assets/Exams/
├── Exam01/   视频开场与战斗闯关（双场景、怪物、技能、门切换）
├── Exam02/   宝箱房间探索（碰撞、宝箱、视频、特效）
├── Exam03/   双平台家园防御（桥、敌人生成、攻击、胜负判定）
├── Exam04/   RPG持久化与战斗（PlayerPrefs、多场景、技能、残血变红）
├── Exam05/   怪物战斗与场景切换（5怪、左键攻击、门、视频）
├── Exam06/   射击蘑菇冒险（三场景、子弹、红绿蘑菇、体型/血量变化）
├── Exam07/   波次防御战斗（空格切场景、寻路、U/I动画、胜负输出）
├── Exam08/   植物大战僵尸（五路线、豌豆射手、敌人波次）
├── Exam09/   赛车竞速（AutomobileRacing、AI赛车、爆炸、终点视频）
└── Exam10/   多场景RPG战斗（左右移动、子弹、巡逻警戒、技能）

每套题内部子目录
----------------
Docs/        教程.md、分步教程.md、试题原文.txt
Scenes/      场景文件（建议命名 ExamXX_Scene1.unity）
Scripts/     C# 脚本（含 ExamXX.asmdef 程序集隔离）
Prefabs/     预制体
Models/      3D 模型
Materials/   材质
Textures/    贴图
Animations/  动画
Audio/       音频
Video/       视频
Effects/     粒子与特效
UI/          UI 资源
Resources/   动态加载资源（必须使用 Resources/ExamXX/ 子路径）

隔离规则（重要）
----------------
1. 脚本：每套题脚本只放在对应 ExamXX/Scripts/，使用 ExamXX 命名空间
2. 场景：每套题场景只放在对应 ExamXX/Scenes/，不要跨目录引用
3. Resources：Load 路径必须带 ExamXX 前缀，如 Resources.Load("Exam01/Player")
4. 预制体/材质：只在当前 Exam 目录内引用，不要与其他 Exam 共享
5. 每套题通过 .asmdef 独立编译，同名类不会互相冲突

原始试卷位置
------------
Tests/考试试卷/考试试题1.doc ~ 考试试题10.doc
