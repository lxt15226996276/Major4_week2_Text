using System.Collections;
using System.Collections.Generic;
using Exam.Exam03;
using UnityEngine;
namespace Exam.Exam08
{
    /// <summary>
    /// 豌豆射手：每1秒发射一颗豌豆，攻击力1
    /// </summary>
    public class PeaShooter : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;  // 拖 PeaBullet 预制体
        [SerializeField] private Transform firePoint;       // 拖子物体 FirePoint
        [SerializeField] private float fireInterval = 1f;    // 考试要求：1秒/发
        [SerializeField] private int damage = 1;           // 考试要求：攻击力1

        [SerializeField] private int maxHp = 10;
        private int currentHp;
        private bool isDead;
        void Awake()
        {
            currentHp = maxHp;
        }
        private void Start()
        {
            // 游戏开始立刻打第一发，之后每1秒一发
            InvokeRepeating(nameof(Fire), 0f, fireInterval);
        }
        /// <summary>
        /// 发射豌豆子弹
        /// </summary>
        private void Fire()
        {
            // 在 FirePoint 的位置和朝向生成子弹
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            // 把伤害值传给子弹
            PeaBullet peaBullet = bullet.GetComponent<PeaBullet>();
            if (peaBullet != null)
            {
                peaBullet.SetDamage(damage);
            }
        }

        /// <summary>
        /// 敌人碰到射手时调用
        /// </summary>
        /// <param name="collision"></param>

        void OnTriggerEnter(Collider other)
        {
            Debug.Log(111);
            if (isDead) return;
            if (!other.CompareTag("Enemy")) return;
            //EnemyController enemyController=collision.gameObject.GetComponent<EnemyController>();
            //if(enemyController==null) return;
            Die();
        }

        /// <summary>
        /// 射手死亡并删除
        /// </summary>
        private void Die()
        {
            isDead = true;
            CancelInvoke(nameof(Fire));   // 停止射击
            Destroy(gameObject);          // 考试要求：死亡删除
        }
    }
}

