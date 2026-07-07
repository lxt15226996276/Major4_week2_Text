using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam09
{
    /// <summary>
    /// Exam09 主场景：Dropdown 选关 + Image 黄/蓝/绿 + 进入对应关卡。
    /// [继承 §七] Exam02 选服 index → 场景名表。
    /// [递进] 无 Async Loading · 用 SceneNames 常量 · 第三关胜利在第 6 步。
    /// </summary>
    public class MainController : MonoBehaviour
    {
        [Header("选关 UI")]
        [SerializeField] private Dropdown levelDropdown;
        [SerializeField] private Image levelColorImage;
        [SerializeField] private Text titleText;

        [Header("进入关卡")]
        [SerializeField] private Button btnEnter;

        private readonly Color[] _levelColors =
        {
            Color.yellow,
            Color.blue,
            Color.green
        };

        // index 与 Dropdown Options 0/1/2 必须一致
        private readonly string[] _levelSceneNames =
        {
            SceneNames.Level1,
            SceneNames.Level2,
            SceneNames.Level3
        };

        private void Start()
        {
            if (titleText != null)
                titleText.text = "选择关卡";

            levelDropdown.onValueChanged.AddListener(OnLevelChanged);
            OnLevelChanged(levelDropdown.value);

            btnEnter.onClick.AddListener(OnEnterClick);
        }

        private void OnLevelChanged(int index)
        {
            if (levelColorImage == null || index < 0 || index >= _levelColors.Length)
                return;

            levelColorImage.color = _levelColors[index];
        }

        /// <summary>思路：取 Dropdown.value → _levelSceneNames[index] → LoadScene。</summary>
        private void OnEnterClick()
        {
            int index = levelDropdown.value;
            if (index < 0 || index >= _levelSceneNames.Length)
                return;

            SceneManager.LoadScene(_levelSceneNames[index]);
        }

        private void OnDestroy()
        {
            if (levelDropdown != null)
                levelDropdown.onValueChanged.RemoveListener(OnLevelChanged);
            if (btnEnter != null)
                btnEnter.onClick.RemoveListener(OnEnterClick);
        }
    }
}