using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam01
{
    /// <summary>
    /// 技能1冲击波：碰到enemy造成伤害
    /// </summary>
    public class SkilwaveContorller : MonoBehaviour
    {
        [Header("伤害")]
        [SerializeField] private int damage = 1;
        [SerializeField] private string enemyTag = "Enemy";

        [Header("生命周期")]
        [SerializeField] private float lifeTime = 1.5f;
        void Start()
        {
            //播放完特效自动销毁
            Destroy(gameObject,lifeTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(enemyTag))return;
            EnemyController enemy=other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

}
