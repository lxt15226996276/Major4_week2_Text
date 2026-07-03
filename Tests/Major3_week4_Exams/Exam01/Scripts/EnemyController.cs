using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Exam.Exam01
{
    /// <summary>
    /// 怪物：血量 受击 死亡
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Header("血量")]
        [SerializeField] private int maxHP = 1;

        private int currentHp;
        private bool isDead;
        void Awake()
        {
            currentHp = maxHP;
        }

        /// <summary>
        /// 受到技能伤害
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            currentHp -= damage;
            if (currentHp <= 0)
            {
                Die();
            }
        }
        /// <summary>
        /// 怪物死亡，销毁自身
        /// </summary>
        void Die()
        {
            isDead = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyDied();
            }
            Destroy(gameObject);
        }
    }

}
