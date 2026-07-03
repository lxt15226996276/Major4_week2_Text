using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
namespace Exam.Exam06
{
    public class EnemyController : MonoBehaviour
    {
        [Header("寻逻范围(以出生点为中心)")]
        [SerializeField] private int pointCount = 4;//寻逻点数量
        [SerializeField] private float range = 10f;//寻逻点半径
        private List<Vector3> patrolPoints = new List<Vector3>();//巡逻点列表
        private int patrolIndex = 0;//当前第几个点索引

        private NavMeshAgent agent;
        private Animator animator;
        private Transform player;

        [Header("战斗距离")]
        [SerializeField] private float detectRange = 5f;//多远发现玩家
        [SerializeField] private float attackRange = 3f;//多远攻击玩家
        [SerializeField] private float attackCd = 2f;//攻击间隔
        private float lastAttackTime;//上次攻击时间

        //出生点
        private Vector3 center;
        //上一帧是不是在战斗
        private bool isInCombat = false;
        [Header("血量")]
        [SerializeField] private int maxHp = 60;
        private int currentHp;
        private bool isDead;

        void Awake()
        {
            currentHp = maxHp;
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            //记住出生点的位置
            center = transform.position;
            SpawnPatrolPoints();
            //先去第0个点
            agent.SetDestination(patrolPoints[0]);

        }
        /// <summary>
        /// 随机生成寻逻点
        /// </summary>
        void SpawnPatrolPoints()
        {
            for (int i = 0; i < pointCount; i++)
            {
                float x = Random.Range(-range, range);
                float z = Random.Range(-range, range);
                Vector3 pos = new Vector3(x, 0, z);
                patrolPoints.Add(center + pos);
            }
        }

        void Update()
        {
            if (isDead) return;
            if (player == null) return;
            //播放行走动画
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsWalking", isMoving);

            //算距离，决定寻逻还是战斗
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= detectRange)
            {
                isInCombat = true;
                //战斗逻辑
                DoCombat(distance);
            }
            else
            {
                if (isInCombat)
                {
                    isInCombat = false;
                    agent.isStopped = false;
                    agent.SetDestination(patrolPoints[patrolIndex]);
                }
                //寻逻
                DoPatrol();
            }
        }
        /// <summary>
        /// 寻逻
        /// </summary>
        void DoPatrol()
        {
            //开启导航移动，保证怪物可以正常行走
            agent.isStopped = false;
            //如果剩余的距离大于停止距离，返回
            if (agent.remainingDistance > agent.stoppingDistance) return;
            //到达当前巡逻点，索引+1，切换下一个巡逻点
            patrolIndex++;
            // 如果索引超出巡逻点总数，重置为0，实现循环巡逻
            if (patrolIndex >= patrolPoints.Count)
            {
                patrolIndex = 0;
            }
            //设置新的目标点
            agent.SetDestination(patrolPoints[patrolIndex]);

        }

        /// <summary>
        /// 战斗逻辑
        /// </summary>
        void DoCombat(float distance)
        {
            //进入敌人的攻击范围,向玩家移动
            agent.SetDestination(player.position);
            //还没到攻击范围，继续追击
            if (distance > attackRange)
            {
                agent.isStopped = false;
                return;
            }
            //停止移动
            agent.isStopped = true;
            //转向玩家
            transform.LookAt(player);
            //冷却好了就攻击
            if (Time.time >= lastAttackTime + attackCd)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        /// <summary>
        /// 被子弹打中，扣血
        /// </summary>
        public void TakeDamgae(int damage)
        {
            if (isDead) return;
            int beforeHp = currentHp;
            currentHp -= damage;
            Debug.Log($"怪物血量变化：{beforeHp}->{currentHp}");
            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }
        }
        /// <summary>
        /// 怪物死亡:停止移动，播放死亡动画
        /// </summary>
        void Die()
        {
            isDead = true;
            agent.isStopped = true;
            animator.SetTrigger("Die");
        }
        /// <summary>
        /// 怪物死亡动画播放完成后销毁
        /// </summary>
        private void OnDie()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyDied();
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// 敌人攻击玩家
        /// </summary>
        private void OnAttackPlayer()
        {
            PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(5);
            }
        }
    }

}
