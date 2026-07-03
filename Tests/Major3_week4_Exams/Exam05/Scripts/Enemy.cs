using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam05
{
    /// <summary>
    /// 怪物：受击播放动画
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        private Animator animator;
        private bool isHit;
        private static readonly int HitHash = Animator.StringToHash("Hit");
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        /// <summary>
        /// 受到攻击，播放受击动画
        /// </summary>
        public void TakeHit()
        {
            if (isHit) return;
            isHit = true;
            animator.SetTrigger(HitHash);
        }
        /// <summary>
        /// 怪物死亡逻辑
        /// </summary>
        private void OnDie()
        {
            if (GameManager.Instacne != null)
            {
                GameManager.Instacne.OnEnemyDied();
            }
            Destroy(gameObject);
        }
    }
}

