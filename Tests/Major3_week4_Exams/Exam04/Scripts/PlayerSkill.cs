using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam04
{
    /// <summary>
    /// 玩家技能：鼠标左键释放，帧事件生成冲击波
    /// </summary>
    public class PlayerSkill : MonoBehaviour
    {
        //动画控制器
        private Animator animator;
        //动画参数
        private static readonly int attackHah = Animator.StringToHash("Attack");
        //是否正在攻击
        private bool isAttacking;
        public bool IsAttacking => isAttacking;
        [Header("技能参数")]
        [SerializeField] private GameObject skillPrefab;
        [SerializeField] private Transform skillFirePoint;
        //cd冷却时间
        [SerializeField] private float cd;
        //上一次播放动画的时间
        private float lastPlayTime = -999f;


        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && CanFire())
            {
                PlayeAttactAnimation();
            }
        }
        /// <summary>
        /// 播放攻击动画
        /// </summary>
        void PlayeAttactAnimation()
        {
            isAttacking = true;
            lastPlayTime = Time.time;
            animator.SetTrigger(attackHah);
        }

        /// <summary>
        /// 是否可以发射
        /// </summary>
        bool CanFire()
        {
            return !isAttacking && Time.time >= lastPlayTime + cd;
        }
        /// <summary>
        /// 发射技能
        /// </summary>
        private void OnAttack()
        {
            if (skillPrefab == null || skillFirePoint == null) return;
            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(skillPrefab, skillFirePoint.position, rotation);
        }
        /// <summary>
        /// 动画播放完成停止攻击
        /// </summary>
        private void OnAttackEnd()
        {
            isAttacking = false;
        }
    }
}

