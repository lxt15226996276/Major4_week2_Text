using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Exam.Exam10
{
    public class Bullet : MonoBehaviour
    {
        //存活时间
        private float lifeTime = 3f;
        //飞行速度
        private float speed = 12f;
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
            if (!other.CompareTag("Cylinder")) return;
            //调用敌人受伤逻辑
            //EnemyController enemy = other.GetComponent<EnemyController>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamgae(damage);
            // }
            SceneManager.LoadScene("Exam10_2");
            Destroy(gameObject);
        }
    }
}

