using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam04
{
    /// <summary>
    /// 玩家血量，供敌人攻击时扣血
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHp = 100;
        private int currentHp;
        private bool isDead;
        public bool IsDead=>isDead;
        void Awake()
        {
            currentHp = maxHp;
        }
        /// <summary>
        /// 玩家受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            currentHp -= damage;
            Debug.Log("玩家当前血量：" + currentHp);
            if (currentHp <= 0)
            {
                isDead = true;
                currentHp = 0;
                Time.timeScale = 0f;
                Debug.Log("玩家死亡，游戏结束");
            }
        }
    }
}

