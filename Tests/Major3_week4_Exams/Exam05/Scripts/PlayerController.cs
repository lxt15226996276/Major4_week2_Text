using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam05
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
        //获取发射技能的脚本
        private PlayerSkill playerSkill;
        void Awake()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            playerSkill = GetComponent<PlayerSkill>();
        }

        void Update()
        {
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
    }

}
