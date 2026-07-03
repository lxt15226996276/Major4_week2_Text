using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam04
{
    /// <summary>
    /// 敌人血量、受伤与死亡逻辑
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        //怪物血量
        [SerializeField] private int hp;
        private bool isDead;
        private bool isLowHp;
        //敌人身上的渲染器
        private SkinnedMeshRenderer skinRender;

        void Awake()
        {
            hp = Random.Range(50, 101);
            skinRender = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        /// <summary>
        /// 敌人受到伤害
        /// </summary>
        public void TakeDamgae(int damage)
        {
            if (isDead) return;
            hp -= damage;
            Debug.Log(gameObject.name + "当前血量：" + hp);
            if (hp < 20)
            {
                TurnRed();
            }
            if (hp <= 0)
            {
                isDead = true;
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// 残血变红
        /// </summary>
        void TurnRed()
        {
            if (isLowHp || skinRender == null) return;
            isLowHp = true;
            Material[] mats = skinRender.materials;
            foreach (var item in mats)
            {
                item.color = Color.red;
            }
            skinRender.materials = mats;
        }
    }

}

