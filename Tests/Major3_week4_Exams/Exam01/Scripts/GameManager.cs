using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam01
{
    /// <summary>
    /// 管理胜负：记录怪物数量，全灭后生成传送门
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [Header("传送门")]
        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private Transform doorSpawnPoint;
        //当前怪物存活数量
        private int aliveEnemyCount;
        //是否已经生成传送门
        private bool doorSpawned;

        void Awake()
        {
            Instance = this;
        }
        /// <summary>
        /// 刷怪时调用，记录本次生成的怪物总数
        /// </summary>
        public void RegisterEnemy(int count)
        {
            aliveEnemyCount = count;
        }
        /// <summary>
        /// 怪物死亡时调用，生成传送门
        /// </summary>
        public void OnEnemyDied()
        {
            if (doorSpawned) return;
            aliveEnemyCount--;
            if (aliveEnemyCount <= 0)
            {
                SpawnDoor();
            }
        }
        /// <summary>
        /// 生成传送门
        /// </summary>
        void SpawnDoor()
        {
            if (doorPrefab == null || doorSpawnPoint == null) return;
            doorSpawned = true;
            Instantiate(doorPrefab, doorSpawnPoint.position, doorSpawnPoint.rotation);
        }
    }

}
