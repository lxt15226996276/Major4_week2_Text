using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam03
{
    public class PlayerController : MonoBehaviour
    {
        [Header("移动参数")]
        //前后移动速度
        [SerializeField] private float moveSpeed = 3f;
        //左右旋转速度
        [SerializeField] private float rotationSpeed = 90f;
        //角色碰撞移动组件
        private CharacterController characterController;
        //动画控制器
        private Animator animator;
        //动画参数
        private readonly static int IsWalkingHash = Animator.StringToHash("IsWalking");

        [Header("移动范围限制参数")]
        [SerializeField] private float clampMinX;
        [SerializeField] private float clampMaxX;
        [SerializeField] private float clampMinZ;
        [SerializeField] private float clampMaxZ;
        // Awake 里缓存，别每帧 GetComponent
        private AttackController attackController;


        void Awake()
        {
            //获取组件
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            attackController = GetComponent<AttackController>();
        }

        void Update()
        {

            if (attackController != null && attackController.IsAttacking) return;
            HandleRotation();
            HandleMoveMent();
            UpdateAnimation();
        }
        /// <summary>
        /// AD控制主角旋转
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
            transform.Rotate(0, rotateInput * rotationSpeed * Time.deltaTime, 0);
        }
        /// <summary>
        /// WS控制主角移动
        /// </summary>
        void HandleMoveMent()
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

            //限制移动范围
            Vector3 nextPos = transform.position + moveDirection;
            nextPos.x = Mathf.Clamp(nextPos.x, clampMinX, clampMaxX);
            nextPos.z = Mathf.Clamp(nextPos.z, clampMinZ, clampMaxZ);

            characterController.Move(nextPos - transform.position);
            //ClampToGround();

        }
        /// <summary>
        /// 播放行走动画，否则待机
        /// </summary>
        void UpdateAnimation()
        {
            bool isWalking = Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);

            animator.SetBool(IsWalkingHash, isWalking);
        }
        /// <summary>
        /// 限制玩家的移动范围
        /// </summary>
        void ClampToGround()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, clampMinX, clampMaxX);
            pos.z = Mathf.Clamp(pos.z, clampMinZ, clampMaxZ);
            transform.position = pos;
        }
    }
}

