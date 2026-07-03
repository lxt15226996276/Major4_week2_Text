using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam02
{
    public class ServerSelectController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Dropdown serverDropdown;
        [SerializeField] private Image zoneColorImage;
        [SerializeField] private Text selectedLabel;
        [SerializeField] private Button btnEnter;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text progressText;

        [Header("场景")]
        [SerializeField] private string gameSceneName = "Exam02_Game";

        //存储三个颜色
        private readonly Color[] zoneColors =
        {
            Color.red,
            Color.yellow,
            Color.green
        };
        //存储三个选区
        private readonly string[] zoneNames = { "一区", "二区", "三区" };

        void Start()
        {
            //serverDropdown.ClearOptions();
            //serverDropdown.AddOptions(new List<string>(zoneNames));
            serverDropdown.onValueChanged.AddListener(OnServerChanged);
            btnEnter.onClick.AddListener(() => StartCoroutine(LoadServerSceneAsync()));
            //默认选中一区
            OnServerChanged(0);
        }
        /// <summary>
        /// 切换dropdown 更换右侧颜色和文字
        /// </summary>
        private void OnServerChanged(int index)
        {
            zoneColorImage.color = zoneColors[index];
            selectedLabel.text = $"当前选中：{zoneNames[index]}";
        }
        /// <summary>
        /// 协程异步加载 +进度条
        /// </summary>
        IEnumerator LoadServerSceneAsync()
        {
            loadingPanel.SetActive(true);
            progressSlider.value = 0f;
            AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
            {
                float progress = op.progress / 0.9f;
                progressSlider.value = progress;
                if (progressText != null)
                {
                    progressText.text = $"加载中...{(int)(progress * 100)}%";
                }
                yield return null;
            }

            progressSlider.value = 1f;
            if (progressText != null)
            {
                progressText.text = $"加载中...100%";
            }
            yield return new WaitForSeconds(0.3f);
            op.allowSceneActivation = true;
        }








    }

}

