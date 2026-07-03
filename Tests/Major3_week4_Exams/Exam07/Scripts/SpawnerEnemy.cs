using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam07
{
    public class SpawnerEnemy : MonoBehaviour
    {
        //敌人生成数量
        [SerializeField] private int enemyCount;
        public int EnemyCount => enemyCount;
        private int currentSpawncount;

        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnRange;
        [SerializeField] private float spawnY;
        void Awake()
        {
            enemyCount = 10;
            // Debug.Log("随机生成敌人的数量为：" + enemyCount);
        }
        void Start()
        {
            InvokeRepeating("SpawnEnemy", 0, 2f);
        }

        /// <summary>
        /// 随机生成敌人
        /// </summary>
        void SpawnEnemy()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
            if (currentSpawncount >= enemyCount)
            {
                CancelInvoke("SpawnEnemy");
                return;
            }

            float x = Random.Range(-spawnRange, spawnRange);
            float z = Random.Range(-spawnRange, spawnRange);
            Vector3 pos = new Vector3(x, spawnY, z);
            Instantiate(enemyPrefab, pos, Quaternion.identity);

            currentSpawncount++;
        }
    }

}
