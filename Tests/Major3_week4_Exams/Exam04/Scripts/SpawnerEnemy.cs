using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam04
{
    public class SpawnerEnemy : MonoBehaviour
    {
        [Header("怪物预制体")]
        [SerializeField] private GameObject enemyPrefab;

        [Header("生成位置范围")]
        [SerializeField] private float spawnRange = 10f;
        [Header("敌人数量")]
        [SerializeField] private int enemyCount = 3;

        void Start()
        {
            SpawnerEnemies();
        }
        void SpawnerEnemies()
        {
            if (enemyPrefab == null) return;

            for (int i = 0; i < enemyCount; i++)
            {
                //怪物生成位置
                float spawnX = Random.Range(-spawnRange, spawnRange);
                float spawnZ = Random.Range(-spawnRange, spawnRange);
                Vector3 pos = new Vector3(spawnX, 0, spawnZ);
                //生成
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }

        }
    }

}
