using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam07
{
    public class AnimationInput : MonoBehaviour
    {
        private Animator animator;
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
            if (Input.GetKeyDown(KeyCode.U))
            {
                animator.SetTrigger("SkillU");
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                animator.SetTrigger("SkillI");
            }
        }
    }
}

