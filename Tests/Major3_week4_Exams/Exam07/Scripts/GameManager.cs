using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Exam.Exam07
{
    /// <summary>
    /// 管理战斗胜负：全灭胜利，敌人到点失败
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private SpawnerEnemy spawner;
        private int aliveEnemyCount;
        private bool isGameOver;
        public bool IsGameOver => isGameOver;
        void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            spawner = GameObject.Find("SpawnerEnemy")?.GetComponent<SpawnerEnemy>();
            if (spawner == null) return;
            aliveEnemyCount = spawner.EnemyCount;
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void Victory()
        {
            if (aliveEnemyCount <= 0)
            {
                isGameOver = true;
                if (aliveEnemyCount > 0) return;
                Time.timeScale = 0;
                Debug.Log("胜利");
            }
        }
        /// <summary>
        /// 游戏失败
        /// </summary>
        public void GameOver()
        {
            isGameOver = true;
            Time.timeScale = 0;
            Debug.Log("游戏失败");
        }

        /// <summary>
        /// 敌人死亡
        /// </summary>
        public void EnemyDied()
        {
            if (isGameOver) return;
            aliveEnemyCount--;
            Debug.Log("当前敌人存活的数量：" + aliveEnemyCount);
        }
    }

}

