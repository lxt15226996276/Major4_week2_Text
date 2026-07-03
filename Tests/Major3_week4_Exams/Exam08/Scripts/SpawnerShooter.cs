using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam08
{
    /// <summary>
    /// 点击地面生成豌豆射手
    /// </summary>
    public class SpawnerShooter : MonoBehaviour
    {
        [SerializeField] private GameObject peaShooterPrefab;
        void Update()
        {
            //鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                //从主相机向鼠标屏幕位置发射射线
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                //射线检测collider的物体
                if (Physics.Raycast(ray, out hit, 100, 1 << 6))
                {
                    //在点击位置生成豌豆射手
                    Quaternion rotation = Quaternion.LookRotation(Vector3.back);
                    Instantiate(peaShooterPrefab, hit.point, rotation);
                }
            }
        }
    }
}

