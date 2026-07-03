using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam03
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("需要销毁的敌人数量")]
        [SerializeField] private int needEnemyCount;
        //计数器：计算销毁敌人的数量
        public int destoryEnemyCount;
        //游戏是否结束
        private bool isGameOver;
        public bool IsGameOver => isGameOver;
        void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        public void Victory()
        {
            if (destoryEnemyCount >= needEnemyCount)
            {
                isGameOver = true;
                Debug.Log("游戏结束");
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void HomeGameOver()
        {

            isGameOver = true;
            Debug.Log("游戏结束");
            Time.timeScale = 0f;

        }

    }
}

