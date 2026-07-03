using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam01
{
    /// <summary>
    /// 控制主角：ws前后移动，AD左右旋转，播放行走/待机动画
    /// </summary>
    public class PlayerController : MonoBehaviour
    {

        [Header("移动参数")]
        //前后移动速度
        [SerializeField] private float moveSpeed = 5f;
        //旋转速度
        [SerializeField] private float rotateSpeed = 100f;
        //角色碰撞移动组件
        private CharacterController characterController;
        //动画控制器
        private Animator animator;
        //动画参数
        private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
        void Awake()
        {
            //获取同一物体上的组件，Awake缓存，避免每帧GetComponent
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            var skill1 = GetComponent<SkillController>();
            if (skill1 != null && skill1.IsCasting) return;
            HandleRotation();
            HandleMovement();
            UpdateAnimation();
        }
        /// <summary>
        /// AD控制左右旋转
        /// </summary>
        private void HandleRotation()
        {
            float rotateInput = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                rotateInput = -1f;//向左转
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rotateInput = 1f;//向右转
            }
            //绕Y轴旋转
            transform.Rotate(0f, rotateInput * rotateSpeed * Time.deltaTime, 0f);
        }
        /// <summary>
        /// ws控制沿面朝方向前后移动
        /// </summary>
        private void HandleMovement()
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

            Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.deltaTime;
            characterController.Move(moveDirection);
        }
        /// <summary>
        /// 按下wads任意键播放行走动画，否则待机
        /// </summary>
        private void UpdateAnimation()
        {
            bool isWalking = Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.S);

            animator.SetBool(IsWalkingHash, isWalking);
        }

    }
}

