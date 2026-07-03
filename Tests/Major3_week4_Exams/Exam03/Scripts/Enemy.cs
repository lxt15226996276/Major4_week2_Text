using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Exam.Exam03
{
    /// <summary>
    /// 三种不同的敌人
    /// </summary>
    public enum EnemyType
    {
        Small,
        Medium,
        Boss

    }
    public class Enemy : MonoBehaviour
    {
        //当前怪物类型
        public EnemyType enemy = EnemyType.Small;
        [Header("怪物血量")]
        [SerializeField] private int maxHp;
        //当前血量
        private int currentHp;

        //目标位置
        [SerializeField] private Transform target;
        //ai 导航代理组件
        private NavMeshAgent agent;

        private bool isDead;
        void Awake()
        {
            currentHp = maxHp;
            agent = GetComponent<NavMeshAgent>();
            target = GameObject.Find("Home")?.transform;
        }

        void Start()
        {
            if (target == null || agent == null) return;
            //朝主角家移动
            agent.SetDestination(target.position);
        }

        /// <summary>
        /// 怪物受伤害的方法
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            currentHp -= damage;
            Debug.Log("当前血量：" + currentHp);
            if (currentHp <= 0)
            {
                isDead = true;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.destoryEnemyCount++;
                    GameManager.Instance.Victory();
                }
                Destroy(gameObject);
            }
        }
    }
}

