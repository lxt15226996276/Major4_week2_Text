using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam03
{
    public class AttackController : MonoBehaviour
    {
        [Header("技能资源")]
        [SerializeField] private GameObject cleavePrefab;//冲击波
        [SerializeField] private Transform firePoint;//发射点
        //动画控制器                                          
        private Animator animator;
        //动画参数
        private readonly static int AttackHash = Animator.StringToHash("Attack");
        //是否正在攻击状态
        private bool isAttacking;
        public bool IsAttacking => isAttacking;
        //冷却时间
        [SerializeField] private float cd = 1.5f;
        //上一次发射的时间
        private float lastTime = -1f;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            //按下鼠标左键发射技能:只播放动画，技能有帧事件完成
            if (Input.GetMouseButtonDown(0) && CanAttack())
            {
                PlayAttack();
            }
        }
        /// <summary>
        /// 是否可以播放技能
        /// </summary>
        /// <returns></returns>
        private bool CanAttack()
        {
            bool isCan = !isAttacking && Time.time >= lastTime + cd;
            return isCan;
        }
        /// <summary>
        /// 播放攻击动画
        /// </summary>
        void PlayAttack()
        {
            isAttacking = true;
            lastTime = Time.time;
            animator.SetTrigger(AttackHash);
        }

        /// <summary>
        /// 发射冲击波
        /// </summary>
        public void OnAttack()
        {
            if (cleavePrefab == null || firePoint == null) return;
            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(cleavePrefab, firePoint.position, rotation);
        }
        /// <summary>
        /// 动画结束
        /// </summary>
        public void OnAttackEnd()
        {
            isAttacking = false;
        }
    }
}

