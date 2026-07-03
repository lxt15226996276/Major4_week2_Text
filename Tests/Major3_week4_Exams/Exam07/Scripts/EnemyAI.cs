using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Exam.Exam07
{
    /// <summary>
    /// 敌人出生后自动寻路到目标点
    /// </summary>
    public class EnemyAI : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Transform targetPoint;
        private Animator animator;
        private bool hasArried;//是否已经到达
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        void Start()
        {
            GameObject target = GameObject.Find("TargetPoint");
            if (target == null) return;
            targetPoint = target.transform;
            agent.SetDestination(targetPoint.position);
        }
        void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
            if (hasArried) return;

            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsWalking", isMoving);

            //路径计算完成且里目标足够仅就停下
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                hasArried = true;
                agent.isStopped = true;
                animator.SetBool("IsWalking", false);
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

}
