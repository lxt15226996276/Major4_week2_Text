using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Exam.Exam05
{
    /// <summary>
    /// 管理怪物数量，全灭后生men
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        //单利
        public static GameManager Instacne { get; private set; }
        [Header("传送门")]
        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private Transform doorSpawnPoint;

        private int aliveEnemyCount;
        private bool doorSpawned;


        void Awake()
        {
            Instacne = this;
        }
        void Start()
        {
            //统计场景中怪物的数量
            aliveEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        /// <summary>
        /// 怪物死亡时候调用
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
            doorSpawned=true;
            Instantiate(doorPrefab,doorSpawnPoint.position,doorSpawnPoint.rotation);
        }
    }
}

