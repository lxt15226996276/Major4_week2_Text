using UnityEngine;
namespace Exam.Exam04
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float turnSpeed;
        [SerializeField] private Animator animator;

        [Header("开火（Skill1 动画事件 OnAttack 触发）")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        private readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

        private bool _isAttacking;
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(h, 0f, v);
            // if (move.sqrMagnitude > 1f)
            //     move.Normalize();
            // transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);
            //bool isMove = move.sqrMagnitude > 0.0001f;
            transform.Translate(new Vector3(0, 0, v) * moveSpeed * Time.deltaTime, Space.Self);
            transform.Rotate(0, h * turnSpeed * Time.deltaTime, 0);

            bool isMove = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
            animator.SetBool(IsWalkingHash, isMove);
        }

        public void OnAttack()
        {
            if (bulletPrefab == null || firePoint == null) return;
            Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        }
        public void TryAttack()
        {
            if (_isAttacking || animator == null) return;
            _isAttacking = true;
            animator.SetTrigger(AttackHash);
        }
        public void OnAttackEnd()
        {
            _isAttacking = false;
        }
    }
}

