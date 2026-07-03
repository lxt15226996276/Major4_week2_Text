using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam04
{
    public class PlayerController : MonoBehaviour
    {
        [Header("移动参数")]
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float moveSpeed = 5f;

        //玩家控制器
        private CharacterController characterController;
        //动画控制器
        private Animator animator;
        //动画参数
        private readonly static int isWalkingHash = Animator.StringToHash("IsWalking");
        //获取技能发射脚本
        private PlayerSkill playerSkill;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            playerSkill = GetComponent<PlayerSkill>();
        }
        void Update()
        {
            if (playerSkill != null && playerSkill.IsAttacking) return;
            HandleRotation();
            HandleMove();
            PlayAnimation();
        }
        /// <summary>
        /// 控制玩家旋转
        /// </summary>
        void HandleRotation()
        {
            float rotateInput = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                rotateInput = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rotateInput = 1f;
            }
            transform.Rotate(0, rotateInput * turnSpeed * Time.deltaTime, 0);
        }
        /// <summary>
        /// 控制玩家前进后退
        /// </summary>
        void HandleMove()
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
            Vector3 moveDection = transform.forward * moveInput * moveSpeed * Time.deltaTime;
            characterController.Move(moveDection);
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        void PlayAnimation()
        {
            bool isPlay = Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);
            animator.SetBool(isWalkingHash, isPlay);
        }
    }
}

