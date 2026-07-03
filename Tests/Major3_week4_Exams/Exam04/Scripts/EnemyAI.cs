using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam04
{
    /// <summary>
    /// 敌人Ai：靠近玩家5米时看向并攻击
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        [Header("攻击参数")]
        [SerializeField] private float attackRange = 5f;
        [SerializeField] private float attackInterval = 1.5f;
        [SerializeField] private int damage = 10;
        private float lastAttacTime = -1f;
        //玩家位置
        private Transform player;
        //动画控制器
        private Animator animator;
        //动画参数
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        //玩家血量
        private PlayerHealth playerHealth;

        void Awake()
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            if (player == null) return;
            float distacne = Vector3.Distance(transform.position, player.position);
            if (distacne <= attackRange)
            {
                LookAtPlayer();
                TryAttackPlayr();
            }
        }
        /// <summary>
        /// 看向玩家
        /// </summary>
        void LookAtPlayer()
        {
            if (playerHealth != null && playerHealth.IsDead) return;
            Vector3 targetPos = player.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }
        /// <summary>
        /// 按间隔发动攻击
        /// </summary>
        void TryAttackPlayr()
        {
            //玩家死后不再攻击
            if (playerHealth != null && playerHealth.IsDead) return;
            if (Time.time < lastAttacTime + attackInterval) return;
            lastAttacTime = Time.time;
            animator.SetTrigger(AttackHash);
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

        }

        private void OnAttack()
        {

        }
        private void OnAttackEnd()
        {

        }
    }

}
