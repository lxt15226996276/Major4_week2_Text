using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam08
{
    public class PeaBullet : MonoBehaviour
    {
        private float speed = 10f;       // 子弹飞行速度
        private float lifeTime = 3f;     // 3秒没打到自动销毁
        private int damage = 1;          // 默认伤害1
        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }
        private void Update()
        {
            // 沿自身前方（+Z）飞行
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

        /// <summary>
        /// 由 PeaShooter 设置伤害值
        /// </summary>
        public void SetDamage(int value)
        {
            damage = value;
        }
        private void OnTriggerEnter(Collider other)
        {
            // 只打敌人
            if (!other.CompareTag("Enemy")) return;
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}

