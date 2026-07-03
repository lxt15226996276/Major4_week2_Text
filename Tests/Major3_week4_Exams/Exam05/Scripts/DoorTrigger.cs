using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Exam.Exam05
{
    /// <summary>
    /// 玩家进入传送门后切换场景
    /// </summary>
    public class DoorTrigger : MonoBehaviour
    {

        //只触发一次
        private bool isTriggered;
        //场景参数
        private string targetSceneName = "Exam05_VideoScene";
        void OnTriggerEnter(Collider other)
        {
            if (isTriggered) return;
            if (!other.CompareTag("Player")) return;
            isTriggered = true;
            SceneManager.LoadScene(targetSceneName);
        }
    }

}
