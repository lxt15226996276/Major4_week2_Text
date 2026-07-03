using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

namespace Exam.Exam05
{
    /// <summary>
    /// 攻击特效：碰到怪物后消失，并让怪物受击
    /// </summary>
    public class SkillCleaveController : MonoBehaviour
    {
        //只打击一次
        private bool hasHit;
        //存活时间
        private float lifeTime = 1.5f;
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
            enemy.TakeHit();
            Destroy(gameObject, 1f);
        }
    }
}

