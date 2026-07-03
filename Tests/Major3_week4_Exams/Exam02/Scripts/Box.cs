using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
namespace Exam.Exam02
{
    public class Box : MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        [SerializeField] private GameObject effectPrefab;

        void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            videoPlayer.loopPointReached += VideoEnd;
            videoPlayer.Play();
        }

        void VideoEnd(VideoPlayer vp)
        {
            SpawnEffect();

            videoPlayer.Stop();

        }
        /// <summary>在宝箱位置生成光芒特效</summary>
        private void SpawnEffect()
        {
            if (effectPrefab == null) return;
            GameObject prefab = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(prefab, 2.5f);

        }
    }
}

