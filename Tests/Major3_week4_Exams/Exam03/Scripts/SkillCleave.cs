using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam03
{
    public class SkillCleave : MonoBehaviour
    {

        //伤害值
        [SerializeField] private int attakDamage = 20;
        [SerializeField] private float lifeTime = 1.5f;

        //是否已经打到敌人
        private bool hasHit;
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            if (!other.CompareTag("Enemy")) return;
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;
            hasHit = true;
            enemy.TakeDamage(attakDamage);

            GetComponent<Collider>().enabled = false;

            //Destroy(gameObject);
        }
    }

}

