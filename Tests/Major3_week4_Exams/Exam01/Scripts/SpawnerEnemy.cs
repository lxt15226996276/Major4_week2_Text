using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam01
{
    public class SpawnerEnemy : MonoBehaviour
    {
        [Header("怪物预制体")]
        [SerializeField] private GameObject enemyPrefab;
        [Header("数量范围")]
        [SerializeField] private int minCount = 2;
        [SerializeField] private int maxCount = 5;

        [Header("生成位置范围")]
        [SerializeField] private float spawnRange = 10f;

        void Start()
        {
          
            SpawnerEnemies();
        }
        void SpawnerEnemies()
        {
            if (enemyPrefab == null) return;
            //怪物生成随机数量 
            int count = Random.Range(minCount, maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                //怪物生成位置
                float spawnX = Random.Range(-spawnRange, spawnRange);
                float spawnZ = Random.Range(-spawnRange, spawnRange);
                Vector3 pos = new Vector3(spawnX, 0, spawnZ);
                //生成
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
            //告诉管理器 本局一共几只怪物
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterEnemy(count);
            }

        }

    }
}

