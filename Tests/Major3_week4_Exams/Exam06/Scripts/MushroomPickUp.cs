using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam06
{
    /// <summary>
    /// 蘑菇类型
    /// </summary>
    public enum MushRoomType
    {
        Red,
        Green
    }
    /// <summary>
    /// 挂在红绿蘑菇预制体上
    /// </summary>
    public class MushroomPickUp : MonoBehaviour
    {
        [SerializeField] private MushRoomType mushRoomType;
        [SerializeField] private int greenDamge = 10;
        /// <summary>
        /// 右键点中后调用
        /// </summary>
        public void PickUp()
        {
            PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            if (player == null) return;

            //根据蘑菇不同类型执行不同效果
            switch (mushRoomType)
            {
                case MushRoomType.Red:
                    player.GrowByRedMushroon();
                    break;
                case MushRoomType.Green:
                    player.TakeDamage(greenDamge);
                    break;
            }
            Debug.Log($"拾取了{mushRoomType}蘑菇");
            Destroy(gameObject);
        }
    }
}

