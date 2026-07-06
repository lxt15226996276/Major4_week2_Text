using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam04
{
    public class LoadingController : MonoBehaviour
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text tipProgressText;
        private float duration = 5f;
        private Coroutine _loadRotuine;
        private void Start()
        {
            progressSlider.value = 0;
            _loadRotuine = StartCoroutine(LoadingProgress());
        }
        IEnumerator LoadingProgress()
        {
            var op = SceneManager.LoadSceneAsync(SceneNames.Main);
            op.allowSceneActivation = false;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float fake = t / duration;
                float real = op.progress / 0.9f;
                float progress = Mathf.Clamp01(Mathf.Min(fake, real));
                progressSlider.value = progress;
                tipProgressText.text = "加载中..." + (int)(progress * 100) + "%";
                yield return null;
            }

            progressSlider.value = 1;
            tipProgressText.text = "加载中..." + 100 + "%";
            yield return new WaitForSeconds(0.3f);
            op.allowSceneActivation = true;
        }
        void OnDestroy()
        {
            if (_loadRotuine != null)
            {
                StopCoroutine(_loadRotuine);
            }
        }
    }

}
