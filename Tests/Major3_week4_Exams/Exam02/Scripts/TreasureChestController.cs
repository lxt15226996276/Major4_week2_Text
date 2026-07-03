using UnityEngine;
using UnityEngine.Video;

namespace Exam.Exam02
{
    /// <summary>
    /// 玩家碰到宝箱触发器后：播放宝藏视频，视频结束后在宝箱位置生成特效。
    /// </summary>
    public class TreasureChestController : MonoBehaviour
    {
        [Header("视频（全屏播放在 Main Camera 上）")]
        [SerializeField] private VideoPlayer videoPlayer;

        [Header("视频结束后生成的特效预制体")]
        [SerializeField] private GameObject effectPrefab;

        private bool opened;

        private void OnTriggerEnter(Collider other)
        {
            if (opened) return;
            if (!other.CompareTag("Player")) return;

            opened = true;
            OpenTreasure();
        }

        /// <summary>触发宝藏开启事件</summary>
        private void OpenTreasure()
        {
            Debug.Log("宝箱开启！");

            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached += OnVideoFinished;
                videoPlayer.Play();
                return;
            }

            SpawnEffect();
        }

        /// <summary>视频播放完毕</summary>
        private void OnVideoFinished(VideoPlayer vp)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.Stop();
            SpawnEffect();
        }

        /// <summary>在宝箱位置生成光芒特效</summary>
        private void SpawnEffect()
        {
            if (effectPrefab == null) return;
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }
    }
}
