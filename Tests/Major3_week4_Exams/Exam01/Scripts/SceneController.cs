using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exam.Exam01
{
    public class SceneController : MonoBehaviour
    {
        void Start()
        {
            //五秒跳转场景
            Invoke("SwitchScene", 5f);
        }

        /// <summary>
        /// 跳转到场景2
        /// </summary>
        private void SwitchScene()
        {
            SceneManager.LoadScene("Exam_Scene2");
        }
    }
}


