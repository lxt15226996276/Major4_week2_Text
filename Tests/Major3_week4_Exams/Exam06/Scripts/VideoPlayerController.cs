using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
namespace Exam.Exam06
{
    public class VideoPlayerController : MonoBehaviour
    {
        //获取视频管理器
        private VideoPlayer videoPlayer;
        void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        void Start()
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }

        void OnVideoEnd(VideoPlayer vp)
        {
            SceneManager.LoadScene("Exam06_Scene2");
        }
    }

}
