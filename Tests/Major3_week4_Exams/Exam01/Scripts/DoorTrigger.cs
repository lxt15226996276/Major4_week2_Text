using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Exam.Exam01
{
    /// <summary>
    /// 玩家进入传送门后播放特效和音效，3秒后回到场景1
    /// </summary>
    public class DoorTrigger : MonoBehaviour
    {
        [Header("进门特效")]
        [SerializeField] private GameObject enterEffectPrefab;
        [Header("音效")]
        [SerializeField] private AudioClip enterSound;
        [Header("场景跳转")]
        [SerializeField] private string targetSceneName = "Exam_Scene1";
        [SerializeField] private float delaySecond = 3f;

        private bool isTriggered;
        private AudioSource audioSource;

        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isTriggered) return;
            if (!other.CompareTag("Player")) return;

            isTriggered = true;
            //生成进门特效
            if (enterEffectPrefab != null)
            {
                Instantiate(enterEffectPrefab, transform.position, transform.rotation);
            }
            //播放音效
            if (enterSound != null)
            {
                audioSource.PlayOneShot(enterSound);
            }
            //三秒后回场景1
            Invoke("GoToScene1", delaySecond);
        }
        private void GoToScene1()
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}

