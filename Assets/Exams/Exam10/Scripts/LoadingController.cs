using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam10
{
    /// <summary>
    /// Exam10 通用 Loading：假进度 + Async 真进度取 Min，结束后进 GameSession.NextScene。
    /// [继承 Exam08] 同一张 Loading 场景用两次（Login 后 / 选服后）。
    /// </summary>
    public class LoadingController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text progressTipText;
        [SerializeField] private float duration = 5f;

        private Coroutine _loadRoutine;

        private void Start()
        {
            progressSlider.value = 0f;
            _loadRoutine = StartCoroutine(LoadNextScene());
        }

        /// <summary>
        /// 思路：Async 加载 NextScene · Slider=Min(假进度, 真进度/0.9) · 满 duration 后激活。
        /// </summary>
        private IEnumerator LoadNextScene()
        {
            string target = GameSession.NextScene;
            var op = SceneManager.LoadSceneAsync(target);
            op.allowSceneActivation = false;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float fake = t / duration;
                float real = op.progress / 0.9f;
                float progress = Mathf.Clamp01(Mathf.Min(fake, real));

                progressSlider.value = progress;
                progressTipText.text = "加载中   " + (int)(progress * 100) + "%";
                yield return null;
            }

            progressSlider.value = 1f;
            progressTipText.text = "加载中   100%";
            yield return new WaitForSeconds(0.2f);
            op.allowSceneActivation = true;
        }

        private void OnDestroy()
        {
            if (_loadRoutine != null)
                StopCoroutine(_loadRoutine);
        }
    }
}