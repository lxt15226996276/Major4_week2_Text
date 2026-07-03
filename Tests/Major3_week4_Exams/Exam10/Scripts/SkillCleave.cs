using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam10
{
    public class SkillCleave : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            EnemyController enemy = other.GetComponent<EnemyController>();
            enemy.Die();
        }
    }
}

