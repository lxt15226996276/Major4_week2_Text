using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam06
{
    /// <summary>
    /// 加载场景 静音按钮 
    /// </summary>
    public class LoadingController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Button btnMute;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text tipProgressText;
        [SerializeField] private Text tipBottomText;

        [Header("Sprites 引用")]
        [SerializeField] private Sprite[] musicSprites;
        private bool isMute;
        private float progress = 0f;
        //过渡时间
        private float duration = 6f;
        //下一个场景名
        private readonly string nextSceneName = "Exam06_Game";

        void Start()
        {
            isMute = audioSource.mute;
            btnMute.onClick.AddListener(OnMuteBtnClick);
            StartCoroutine(LoadingScene());
        }

        IEnumerator LoadingScene()
        {
            var op = SceneManager.LoadSceneAsync(nextSceneName);
            op.allowSceneActivation = false;
            float t = 0f;
            while (t <= duration)
            {
                t += Time.deltaTime;
                float fake = t / duration;
                float real = op.progress / 0.9f;
                progress = Mathf.Clamp01(Mathf.Min(fake, real));
                progressSlider.value = progress;
                ReFreshUI();
                yield return null;
            }
            progress = 1f;
            progressSlider.value = progress;
            ReFreshUI();
            yield return new WaitForSeconds(0.3f);
            op.allowSceneActivation = true;
        }
        /// <summary>
        /// 静音
        /// </summary>
        private void OnMuteBtnClick()
        {
            audioSource.mute = !audioSource.mute;
            isMute = audioSource.mute;
            ReFreshUI();
        }

        /// <summary>
        /// 跟新UI 显示
        /// </summary>
        private void ReFreshUI()
        {
            btnMute.image.sprite = isMute ? musicSprites[0] : musicSprites[1];
            btnMute.GetComponentInChildren<Text>().text = isMute ? "静音" : "开启";

            tipProgressText.text = "加载中  " + (int)(progress * 100) + "%";
            tipBottomText.text = GetBottomTip(progress);
        }
        /// <summary>
        /// 根据进度刷新提示文字
        /// </summary>
        /// <returns></returns>
        private string GetBottomTip(float progress)
        {
            if (progress < 0.33f) return "正在加载服务器...";
            if (progress < 0.66f) return "正在加载游戏场景...";
            return "即将进入游戏...";
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        void OnDestroy()
        {
            if (btnMute != null) btnMute.onClick.RemoveListener(OnMuteBtnClick);
        }

    }
}

