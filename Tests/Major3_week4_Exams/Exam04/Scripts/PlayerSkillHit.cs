using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam04
{
    /// <summary>
    /// 玩家技能冲击波：碰到敌人扣血
    /// </summary>
    public class PlayerSkillHit : MonoBehaviour
    {
        [SerializeField] private int damage = 30;
        [SerializeField] private float lifeTime = 1.5f;

        private bool hasHit;
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            if (hasHit) return;
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;
            hasHit = true;
            enemy.TakeDamgae(damage);
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

}

