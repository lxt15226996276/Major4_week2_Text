using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam10
{
    public class PlayerController : MonoBehaviour
    {

        [Header("移动参数")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float turnSpeed;

        //获取玩家控制器和动画控制器
        private CharacterController characterController;
        private Animator animator;
        private bool isAttacking;

        [Header("动画参数")]
        private static readonly int IsWaklingHash = Animator.StringToHash("IsWalking");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int DieadHash = Animator.StringToHash("Die");
        [SerializeField] private float attackInterval;//攻击间隔
        private float lastAttackTime = -1f;//上一次攻击时间
        [Header("技能参数")]
        [SerializeField] private GameObject skillPrefab;
        [SerializeField] private Transform skillFirePoint;
        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            //if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

            if (isAttacking) return;
            HandleMonvement();
            HandleTurnRotation();
            PlayMoveAnimation();

            //鼠标左键点击生成子弹
            if (Input.GetMouseButtonDown(0) && CanAttack())
            {
                PlayAttackAnimation();
            }

        }
        /// <summary>
        /// 前后移动
        /// </summary>
        void HandleMonvement()
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
        /// 左右旋转
        /// </summary>
        void HandleTurnRotation()
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
        /// 开启移动动画
        /// </summary>
        void PlayMoveAnimation()
        {
            bool isWalking = Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);
            animator.SetBool(IsWaklingHash, isWalking);
        }


        /// <summary>
        /// 开启攻击动画
        /// </summary>
        void PlayAttackAnimation()
        {
            animator.SetTrigger(AttackHash);
            isAttacking = true;
            lastAttackTime = Time.time;
        }
        /// <summary>
        /// 攻击逻辑
        /// </summary>
        private void OnAttack()
        {
            //生成子弹
            if (skillPrefab == null || skillFirePoint == null) return;
            Quaternion rotation = Quaternion.LookRotation(transform.forward);
            Instantiate(skillPrefab, skillFirePoint.position, rotation);
        }
        /// <summary>
        /// 攻击动画播放完成
        /// </summary>
        private void OnAttackEnd()
        {
            isAttacking = false;
        }
        /// <summary>
        /// 是否能够发起攻击
        /// </summary>
        /// <returns></returns>
        public bool CanAttack()
        {
            return !isAttacking && Time.time >= lastAttackTime + attackInterval;
        }
    }

}
