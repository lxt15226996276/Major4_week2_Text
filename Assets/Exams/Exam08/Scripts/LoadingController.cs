using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam08
{
    /// <summary>
    /// Exam08 通用 Loading：假进度 + Async 真进度取 Min，结束后激活目标场景。
    /// </summary>
    public class LoadingController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text progressTipText;
        [SerializeField] private float duration = 5f; // 假进度时长，5～6s 均可
        void Start()
        {
            progressSlider.value = 0f;
            StartCoroutine(LoadNextScene());
        }

        /// <summary>
        /// 协程：Async 加载 NextScene，Slider 显示 Min(假进度, 真进度/0.9)。
        /// </summary>
        private IEnumerator LoadNextScene()
        {
            string target = GameSession.NextScene;
            var op = SceneManager.LoadSceneAsync(target);
            op.allowSceneActivation = false; // 进度到 1 前不切换，避免闪屏

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float fake = t / duration;
                float real = op.progress / 0.9f; // Unity Async 最大约 0.9
                float progress = Mathf.Clamp01(Mathf.Min(fake, real));
                progressSlider.value = progress;
                progressTipText.text = "加载中   " + (int)(progress * 100) + "%";
                yield return null;
            }

            progressSlider.value = 1f;
            progressTipText.text = "加载中   " + 100 + "%";
            yield return new WaitForSeconds(0.2f);
            Debug.Log("loading" + GameSession.NextScene);
            op.allowSceneActivation = true;

        }
    }
}