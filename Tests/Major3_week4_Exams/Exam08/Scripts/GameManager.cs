using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam08
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private int killCount = 0;           // 已击杀敌人数
        private const int WIN_KILL_COUNT = 10; // 考试要求：死10个胜利
        private bool isGameOver;
        public bool IsGameOver => isGameOver;
        private void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 敌人被击杀时调用（EnemyController.Die 里调）
        /// </summary>
        public void EnemyDied()
        {
            if (isGameOver) return;
            killCount++;
            Debug.Log("当前击杀数：" + killCount);
            if (killCount >= WIN_KILL_COUNT)
            {
                Victory();
            }
        }
        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void Victory()
        {
            if (isGameOver) return;
            isGameOver = true;
            Time.timeScale = 0;
            Debug.Log("胜利");
        }
        /// <summary>
        /// 游戏失败（敌人到达终点）
        /// </summary>
        public void GameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            Time.timeScale = 0;
            Debug.Log("游戏失败");
        }

    }
}

