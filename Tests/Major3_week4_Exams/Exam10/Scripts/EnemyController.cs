using System.Collections;
using System.Collections.Generic;
using Exam.Exam09;
using UnityEngine;
using UnityEngine.AI;
namespace Exam.Exam10
{
    public class EnemyController : MonoBehaviour
    {

        //巡逻点
        [SerializeField] private List<Transform> patrolPoints;
        //当前寻逻点索引
        private int patrolIndex;
        //导航代理组件
        private NavMeshAgent agent;
        private Animator animator;
        private Transform player;
        //寻逻范围
        [SerializeField] private float patrolRange = 5f;
        //攻击范围
        [SerializeField] private float attackRange = 3f;
        //攻击冷却时间
        private float attackCd = 1.5f;
        //上一次攻击时间
        private float lasetAttackTime = -1f;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            patrolIndex = 0;


        }

        void Start()
        {
            //先去第一个寻逻点

            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        void Update()
        {
            //是否播放行走动画
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsWalking", isMoving);
            if (player == null)
            {
                DoPartol();
                return;
            }
            //计算距离并决定寻逻和战斗
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= patrolRange)
            {
                DoCombat(distance);
            }
            else
            {
                DoPartol();
            }
        }
        /// <summary>
        /// 寻逻
        /// </summary>
        void DoPartol()
        {
            agent.isStopped = false;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                patrolIndex++;
                if (patrolIndex >= patrolPoints.Count)
                {
                    patrolIndex = 0;
                }
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }

        }
        /// <summary>
        /// 战斗
        /// </summary>
        void DoCombat(float distance)
        {
            // agent.isStopped = true;
            // animator.SetBool("IsWalking", false);
            // transform.LookAt(player);

            //追击玩家
            agent.SetDestination(player.position);
            //如果没到攻击范围，继续追击
            if (distance > attackRange)
            {
                agent.isStopped = false;
                return;
            }

            agent.isStopped = true;
            transform.LookAt(player.position);
            //攻击
            if (Time.time > lasetAttackTime + attackCd)
            {
                animator.SetTrigger("Attack");
                lasetAttackTime = Time.time;
            }
        }
        /// <summary>
        /// 敌人的死亡逻辑
        /// </summary>
        public void Die()
        {
            animator.SetTrigger("Die");
        }
        /// <summary>
        /// 动画播放完成之后死亡
        /// </summary>
        private void OnDie()
        {
            Destroy(gameObject);
        }

        private void OnAttackPlayer()
        {

        }
    }

}
