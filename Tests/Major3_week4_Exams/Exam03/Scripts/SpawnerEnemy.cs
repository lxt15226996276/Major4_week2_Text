using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam03
{
    public class SpawnerEnemy : MonoBehaviour
    {
        [Header("生成敌人")]
        [SerializeField] private GameObject[] enemyPrefab;
        [SerializeField] private float spawnInterval;

        void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

            //每隔3秒随机生成
            InvokeRepeating("SpawnEnemies", 0, spawnInterval);
        }

        /// <summary>
        /// 生成敌人
        /// </summary>
        void SpawnEnemies()
        {
            int index = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[index], transform.position, Quaternion.identity, transform);
        }


    }
}

