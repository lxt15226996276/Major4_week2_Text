using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam08
{
    /// <summary>
    /// Exam08 主场景：减血同步 Slider，血尽显示胜利 Panel，关闭回登录。
    /// </summary>
    public class MainController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnReduceHp;
        [SerializeField] private GameObject victoryPanel;

        private const int MaxHp = 100;
        private const int ReduceAmount = 20;
        private const string LoginSceneName = "Exam08_LoginScene";

        private int hp;
        private bool isVictory;

        void Start()
        {
            hp = MaxHp;
            hpSlider.maxValue = MaxHp;
            hpSlider.value = hp;
            victoryPanel.SetActive(false);

            btnClose.onClick.AddListener(OnCloseClick);
            btnReduceHp.onClick.AddListener(OnReduceClick);
        }

        /// <summary>
        /// 思路：门闩 → Clamp 减 20 → 同步 Slider → hp≤0 开 VictoryPanel。
        /// </summary>
        private void OnReduceClick()
        {
            if (isVictory) return;

            hp = Mathf.Clamp(hp - ReduceAmount, 0, MaxHp);
            hpSlider.value = hp;

            if (hp <= 0)
            {
                isVictory = true;
                victoryPanel.SetActive(true);
            }
        }

        private void OnCloseClick()
        {
            SceneManager.LoadScene(LoginSceneName);
        }

        void OnDestroy()
        {
            if (btnClose != null) btnClose.onClick.RemoveListener(OnCloseClick);
            if (btnReduceHp != null) btnReduceHp.onClick.RemoveListener(OnReduceClick);
        }
    }
}