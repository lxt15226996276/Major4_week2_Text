using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exam.Exam06
{
    /// <summary>
    /// 随机生成蘑菇+右键拾取
    /// </summary>
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; }
        private int aliveEnemyCount;
        void Awake()
        {
            Instance = this;
        }

        [Header("蘑菇预制体")]
        [SerializeField] private GameObject redMushRoom;
        [SerializeField] private GameObject greenMushRoom;

        [Header("生成参数")]
        [SerializeField] private int spawnCount = 5;
        [SerializeField] private float spawnRange = 15f;
        [SerializeField] private float spawnY;

        void Start()
        {
            aliveEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            SpawnMushroom();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                TryPickMushroom();
            }
        }
        /// <summary>
        /// 随机位置生成蘑菇
        /// </summary>
        void SpawnMushroom()
        {
            for (int i = 0; i < spawnCount; i++)
            {
                float x = Random.Range(-spawnRange, spawnRange);
                float z = Random.Range(-spawnRange, spawnRange);
                Vector3 pos = new Vector3(x, spawnY, z);

                //随机颜色
                bool isRed = Random.Range(0, 2) == 0;
                GameObject roomPrefab = isRed ? redMushRoom : greenMushRoom;
                Instantiate(roomPrefab, pos, Quaternion.identity);
            }
        }
        /// <summary>
        /// 射线检测：点中蘑菇就拾取
        /// </summary>
        void TryPickMushroom()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MushroomPickUp mushroom = hit.transform.GetComponent<MushroomPickUp>();
                if (mushroom != null)
                {
                    mushroom.PickUp();
                }
            }
        }
        /// <summary>
        /// 怪物全部死亡时调用
        /// </summary>
        public void OnEnemyDied()
        {
            aliveEnemyCount--;
            Debug.Log("剩余怪物数量：" + aliveEnemyCount);
            if (aliveEnemyCount <= 0)
            {
                aliveEnemyCount = 0;
                SceneManager.LoadScene("Exam06_Scene3");
            }
        }
    }

}
