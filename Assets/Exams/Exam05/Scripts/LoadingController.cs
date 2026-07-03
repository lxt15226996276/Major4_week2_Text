using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam05
{
    /// <summary>
    /// 加载场景 音乐控制 
    /// </summary>
    public class LoadingController : MonoBehaviour
    {
        [Header("UI  引用")]
        [SerializeField] private Button muteBtn;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text tipProgressText;
        [SerializeField] private Image backGroundImag;
        [SerializeField] private Text tipTextBottom;

        [Header("精灵图片")]
        [SerializeField] private Sprite[] musicSprites;
        [SerializeField] private Sprite backLoadingSprite;
        private float duration = 5f;
        private readonly string loadingNexScene = "Exam05_GameScene";
        void Start()
        {
            //添加事件
            muteBtn.onClick.AddListener(MuteBtnOnClick);
            StartCoroutine(LoadingScene());
        }

        /// <summary>
        /// 静音关闭声音 并切换图片
        /// </summary>
        private void MuteBtnOnClick()
        {
            audioSource.mute = !audioSource.mute;
            muteBtn.image.sprite = audioSource.mute ? musicSprites[1] : musicSprites[0];
            muteBtn.GetComponentInChildren<Text>().text = audioSource.mute ? "静音" : "开启";
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadingScene()
        {
            yield return new WaitForSeconds(0.1f);
            backGroundImag.sprite = backLoadingSprite;
            backGroundImag.color = new Color(1f, 1f, 1f, 0.73f);
            var op = SceneManager.LoadSceneAsync(loadingNexScene);
            op.allowSceneActivation = false;
            float currentTime = 0;
            while (currentTime <= duration)
            {
                currentTime += Time.deltaTime;
                float fake = currentTime / duration;
                float real = op.progress / 0.9f;
                float progress = Mathf.Clamp01(Mathf.Min(fake, real));
                progressSlider.value = progress;
                tipProgressText.text = "加载中 " + (int)(progress * 100) + "%";
                tipTextBottom.text = GetTipByProgress(progress);
                yield return null;
            }

            progressSlider.value = 1;
            tipProgressText.text = "加载中 " + 100 + "%";
            yield return new WaitForSeconds(0.3f);
            op.allowSceneActivation = true;
        }
        /// <summary>
        /// 根据进程显示加载提示
        /// </summary>
        /// <returns></returns>
        private string GetTipByProgress(float t)
        {
            if (t < 0.33f) return "正在连接服务器...";
            if (t < 0.66f) return "正在加载场景...";
            return "即将进入游戏...";
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        void OnDestroy()
        {
            muteBtn.onClick.RemoveListener(MuteBtnOnClick);
        }

    }
}

