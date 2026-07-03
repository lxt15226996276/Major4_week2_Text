using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam08
{
    public class SpawnerEnemy : MonoBehaviour
    {
        // 直接把 5 个 StartPoint 拖进来（Size = 5）
        [SerializeField] private Transform[] spawnPoints;
        // 拖入 Enemy 预制体
        [SerializeField] private GameObject enemyPrefab;
        //生成时间间隔
        [SerializeField] private float spawnInterval;
        void Start()
        {
            InvokeRepeating("SpawnEnemy", 0, spawnInterval);
        }

        /// <summary>
        /// 在随机路线起点生成敌人
        /// </summary>
        private void SpawnEnemy()
        {
            //获取随机出生点
            int index = Random.Range(0, spawnPoints.Length);
            Transform startPoint = spawnPoints[index];
            Vector3 spawnPos = startPoint.position;

            Transform endPoint = startPoint.parent.Find("EndPoint");
            // 生成敌人
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.Init(endPoint);
            }
        }
    }
}

