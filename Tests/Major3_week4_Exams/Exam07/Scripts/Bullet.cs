using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam07
{
    public class Bullet : MonoBehaviour
    {
        //存活时间
        private float lifeTime = 3f;
        //飞行速度
        private float speed = 12f;
        //伤害值
        private int damage = 20;
        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;
            //调用敌人受伤逻辑
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnemyDied();
                GameManager.Instance.Victory();
            }
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}

