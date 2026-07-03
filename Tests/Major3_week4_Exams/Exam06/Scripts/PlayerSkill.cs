using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam06
{
    public class PlayerSkill : MonoBehaviour
    {
        //动画控制器
        private Animator animator;
        //动画参数
        private static readonly int attackHash = Animator.StringToHash("Attack");
        //是否正在攻击
        private bool isAttacking;
        public bool IsAttacking => isAttacking;
        //技能冷却cd
        [SerializeField] private float skillCd;
        //上一次动画播放的时间
        private float lastPlayTime = -1f;
        [Header("技能参数")]
        [SerializeField] private GameObject skillPrefab;
        [SerializeField] private Transform skillFirePoint;
        private PlayerController playerController;

        void Awake()
        {
            playerController = GetComponent<PlayerController>();
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            if (playerController != null && playerController.IsDead) return;
            if (Input.GetMouseButtonDown(0) && IsCanAttack())
            {
                PlayAnimation();
            }
        }


        /// <summary>
        /// 播放动画
        /// </summary>
        void PlayAnimation()
        {
            isAttacking = true;
            lastPlayTime = Time.time;
            animator.SetTrigger(attackHash);
        }
        /// <summary>
        /// 释放技能
        /// </summary>
        private void OnAttack()
        {
            if (skillPrefab == null || skillFirePoint == null) return;
            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(skillPrefab, skillFirePoint.position, rotation);
        }
        /// <summary>
        /// 动画结束逻辑
        /// </summary>
        private void OnAttackEnd()
        {
            isAttacking = false;
        }

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        /// <returns></returns>
        bool IsCanAttack()
        {
            return !isAttacking && Time.time > lastPlayTime + skillCd;
        }
    }

}

