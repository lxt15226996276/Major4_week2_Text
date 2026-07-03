using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam01
{
    /// <summary>
    /// 技能1：按数字键1播放动画并生成冲击波
    /// </summary>
    public class SkillController : MonoBehaviour
    {
        [Header("技能资源")]
        [SerializeField] private GameObject cleavePrefab;//冲击波预制体
        [SerializeField] private Transform spawnPoint;//发射点

        [Header("Animator")]
        [SerializeField] private Animator animator;//动画控制器组件
        private static readonly int Skill1Hash = Animator.StringToHash("Skill1");//动画技能参数

        [Header("冷却")]
        [SerializeField] private float cd = 1.5f;//冷却时间
        private float lastCastTime = -999f;//上一次放技能的时间
        private bool isCasting;//正在释放技能
        public bool IsCasting => isCasting;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            //按下1并且可发射技能
            if (Input.GetKeyDown(KeyCode.Alpha1) && CanCast())
            {
                CastSkill1();
            }
        }
        /// <summary>
        /// 是否可以放技能
        /// </summary>
        /// <returns></returns>
        private bool CanCast()
        {
            bool isCanSkill = !isCasting && Time.time >= lastCastTime + cd;
            return isCanSkill;
        }
        /// <summary>
        /// 按1播放技能动画
        /// </summary>
        private void CastSkill1()
        {
            isCasting = true;
            lastCastTime = Time.time;
            animator.SetTrigger(Skill1Hash);
        }

        /// <summary>
        /// 发射冲击波特效
        /// </summary>
        public void OnSkill1Fire()
        {
            if (cleavePrefab == null || spawnPoint == null) return;

            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(cleavePrefab, spawnPoint.position, rotation);
        }
        /// <summary>
        /// 动画播放完成时由帧事件调用
        /// </summary>
        public void OnSkill1End()
        {
            isCasting = false;
            Debug.Log("技能1播放完成");
        }
    }
}

