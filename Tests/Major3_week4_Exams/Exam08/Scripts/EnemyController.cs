using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Exam.Exam08
{
    /// <summary>
    /// 敌人控制器：血量 攻击 沿路线NavMesh 寻路到终点
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Header("属性")]
        [SerializeField] private int maxHp = 10;      // 血量10
        private int currentHp;
        [SerializeField] private int attack = 5;      // 攻击力5
        public int Attack => attack;
        private bool hasArried;

        private Animator animator;
        private NavMeshAgent agent;
        private Transform targetPoint;   // 这条路线的终点
        private bool isDead;

        void Awake()
        {
            currentHp = maxHp;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            if (isDead) return;
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsWalking", isMoving);

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (hasArried) return;
                hasArried = true;
                agent.isStopped = true;
                animator.SetBool("IsWalking", false);
                // 通知 GameManager 游戏失败
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
                }
                Debug.Log("敌人到达终点");
            }
        }
        /// <summary>
        /// 出生后由SpawnerEnemy调用，传入这条路线的EndPoint
        /// </summary>
        /// <param name="endPoint"></param>
        public void Init(Transform endPoint)
        {
            targetPoint = endPoint;
            agent.SetDestination(targetPoint.position);
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
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
        /// 敌人死亡播放动画
        /// </summary>
        void Die()
        {
            if (isDead) return;   // 防止重复调用
            isDead = true;
            agent.isStopped = true;
            animator.SetTrigger("Die");
            // 通知 GameManager：敌人死亡 +1
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnemyDied();
            }
        }
        /// <summary>
        /// 播放完动画销毁
        /// </summary>
        private void OnDie()
        {
            Destroy(gameObject);
        }
    }
}

