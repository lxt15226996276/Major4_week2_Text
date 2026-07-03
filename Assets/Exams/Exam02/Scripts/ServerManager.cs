using System.Collections;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam02
{
    public class ServerManager : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Dropdown serverDropDown;
        [SerializeField] private Text labelText;
        [SerializeField] private Text progressText;
        [SerializeField] private Image zoneColorImage;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Button btnEnter;




        //三种颜色 三个场景
        private readonly Color[] colors = { Color.red, Color.yellow, Color.green };
        private readonly string[] sceneNames = { "Exam02_Game1", "Exam02_Game2", "Exam02_Game3" };
        private readonly string[] zoneNames = { "一区", "二区", "三区" };
        void Start()
        {
            serverDropDown.onValueChanged.AddListener(SelectedChanged);
            SelectedChanged(serverDropDown.value);
            btnEnter.onClick.AddListener(() => StartCoroutine(LoadingScene(serverDropDown.value)));
        }

        /// <summary>
        /// 选区 变换颜色 加载场景
        /// </summary>
        private void SelectedChanged(int index)
        {
            zoneColorImage.color = colors[index];
            labelText.text = "当前选中： " + zoneNames[index];
        }

        IEnumerator LoadingScene(int index)
        {
            loadingPanel.SetActive(true);

            progressSlider.value = 0;
            var op = SceneManager.LoadSceneAsync(sceneNames[index]);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                float progress = op.progress / 0.9f;//unity progress上限约0.9
                progressSlider.value = progress;
                progressText.text = "加载中...  " + (int)(progress * 100) + "%";
                yield return null;//让出主线程，slider才能每帧重绘
            }
            progressSlider.value = 1;
            progressText.text = "加载中...  " + 100 + "%";
            yield return new WaitForSeconds(0.3f); //评卷肉眼看到满条
            op.allowSceneActivation = true;

        }
        void OnDestroy()
        {
            if (serverDropDown != null) serverDropDown.onValueChanged.RemoveAllListeners();
            if (btnEnter != null) btnEnter.onClick.RemoveAllListeners();
        }
    }

}
