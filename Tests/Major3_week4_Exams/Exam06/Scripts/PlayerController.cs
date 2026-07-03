using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam06
{
    public class PlayerController : MonoBehaviour
    {
        //旋转速度
        private float turnSpeed = 90f;
        //移动速度
        private float moveSpeed = 5f;
        //获取动画控制器和玩家控制器
        private Animator animator;
        private CharacterController characterController;
        //动画参数
        private static readonly int isWalkingHash = Animator.StringToHash("IsWalking");
        private static readonly int deadHash = Animator.StringToHash("Die");
        //获取发射技能的脚本
        private PlayerSkill playerSkill;
        //玩家是否死亡
        private bool isDead;
        public bool IsDead => isDead;
        //玩家血量
        private int maxHp = 100;
        private int currentHp;
        void Awake()
        {
            currentHp = maxHp;
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            playerSkill = GetComponent<PlayerSkill>();
        }

        void Update()
        {
            if (isDead) return;
            if (playerSkill != null && playerSkill.IsAttacking) return;
            HandleRotation();
            HandleMovement();
            PlayAnimation();
        }

        /// <summary>
        /// 左右旋转
        /// </summary>
        void HandleRotation()
        {
            float turnInput = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                turnInput = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                turnInput = 1f;
            }
            transform.Rotate(0, turnInput * turnSpeed * Time.deltaTime, 0);
        }
        /// <summary>
        /// 前后移动
        /// </summary>
        void HandleMovement()
        {
            float moveInput = 0f;
            if (Input.GetKey(KeyCode.W))
            {
                moveInput = 1f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveInput = -1f;
            }
            Vector3 moveDirecton = transform.forward * moveInput * moveSpeed * Time.deltaTime;
            characterController.Move(moveDirecton);
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        void PlayAnimation()
        {
            bool isWalking = Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);
            animator.SetBool(isWalkingHash, isWalking);
        }
        /// <summary>
        /// 玩家受伤
        /// </summary>
        public void TakeDamage(int damge)
        {
            if (isDead) return;
            int beforeHp = currentHp;
            currentHp -= damge;
            Debug.Log($"玩家受到攻击，血量变化：{beforeHp}->{currentHp}");
            if (currentHp <= 0)
            {
                currentHp = 0;
                Die();
            }
        }
        /// <summary>
        /// 玩家死亡逻辑
        /// </summary>
        void Die()
        {
            isDead = true;
            animator.SetTrigger(deadHash);
        }
        /// <summary>
        /// 拾取红蘑菇：体形变大两倍
        /// </summary>
        public void GrowByRedMushroon()
        {
            transform.localScale *= 2f;
            Debug.Log("拾取红蘑菇：体型变大两倍");
        }
    }
}

