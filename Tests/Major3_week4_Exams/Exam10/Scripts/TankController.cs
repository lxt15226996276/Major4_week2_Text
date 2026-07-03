using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam10
{
    public class TankController : MonoBehaviour
    {
        //移动速速
        private float moveSpeed = 6f;
        //子弹预制体
        public GameObject bulletPrefab;
        //发射位置
        public Transform firePoint;

        void Update()
        {
            float H = Input.GetAxis("Horizontal");
            transform.Translate(transform.right * H * moveSpeed * Time.deltaTime, Space.World);

            //右键发射子弹
            if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }
        }
        /// <summary>
        /// 发射子弹
        /// </summary>
        void Fire()
        {
            if (bulletPrefab == null || firePoint == null) return;
            //实例化子弹
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }

    }

}

