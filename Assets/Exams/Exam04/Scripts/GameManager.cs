using UnityEngine;
namespace Exam.Exam04
{
    public class GameManager : MonoBehaviour
    {
        [Header("刷怪")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int enemyCount;
        [SerializeField] private float spawnRangeX;
        [SerializeField] private float spawnRangeY;
        [SerializeField] private float spawnRangeZ;

        [Header("胜利")]
        [SerializeField] private GameObject victoryPanel;
        private int _aliveCount;
        public static GameManager Instance { get; private set; }
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            _aliveCount = enemyCount;
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }
            SpawnEnemies();
        }
        private void SpawnEnemies()
        {
            if (enemyPrefab == null) return;


            for (int i = 0; i < enemyCount; i++)
            {
                float x = Random.Range(-spawnRangeX, spawnRangeX);
                float z = Random.Range(-spawnRangeZ, spawnRangeZ);
                Vector3 pos = new Vector3(x, spawnRangeY, z);
                Instantiate(enemyPrefab, pos, Quaternion.identity);

            }
        }
        public void OnEnemyDied()
        {
            _aliveCount--;
            if (_aliveCount <= 0 && victoryPanel != null)
                victoryPanel.SetActive(true);
        }
    }
}