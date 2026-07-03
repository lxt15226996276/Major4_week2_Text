using UnityEngine;

namespace Exam.Exam03
{
    /// <summary>
    /// Exam03 主场景：随机刷 3 个敌人。敌人总血量 UI 在第 3 步已显示 0，本脚本不改。
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("刷怪")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int enemyCount = 3;
        [SerializeField] private float spawnRangeX = 22.5f;
        [SerializeField] private float spawnRangeZ = 22.5f;
        [SerializeField] private float spawnY = 0.35f;

        private void Start()
        {
            SpawnEnemies();
        }

        /// <summary>
        /// 思路：循环 3 次 Instantiate，X/Z Random，Y 略抬高。
        /// </summary>
        private void SpawnEnemies()
        {
            if (enemyPrefab == null) return;

            for (int i = 0; i < enemyCount; i++)
            {
                float x = Random.Range(-spawnRangeX, spawnRangeX);
                float z = Random.Range(-spawnRangeZ, spawnRangeZ);
                Vector3 pos = new Vector3(x, spawnY, z);
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
        }
    }
}