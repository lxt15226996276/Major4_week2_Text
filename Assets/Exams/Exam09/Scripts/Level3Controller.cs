using UnityEngine;
using UnityEngine.UI;

namespace Exam.Exam09
{
    /// <summary>
    /// 第三关胜利逻辑
    /// </summary>
    public class Level3Controller : MonoBehaviour
    {
        [SerializeField] private Button btnA;
        [SerializeField] private Button btnB;
        [SerializeField] private Button btnC;
        [SerializeField] private GameObject victoryPanel;

        private bool _clickedA;
        private bool _clickedB;
        private bool _clickedC;

        private void Start()
        {
            victoryPanel?.SetActive(false);
            btnA.onClick.AddListener(OnBtnAClick);
            btnB.onClick.AddListener(OnBtnBClick);
            btnC.onClick.AddListener(OnBtnCClick);
        }

        private void OnBtnAClick()
        {
            if (_clickedA) return;  // 点过了，退出
            _clickedA = true;
            CheckWin();
        }

        private void OnBtnBClick()
        {
            if (_clickedB) return;
            _clickedB = true;
            CheckWin();
        }

        private void OnBtnCClick()
        {
            if (_clickedC) return;
            _clickedC = true;
            CheckWin();
        }

        /// <summary>三个 bool 里 true 的个数达到 3 → 胜利。</summary>
        private void CheckWin()
        {
            int n = (_clickedA ? 1 : 0) + (_clickedB ? 1 : 0) + (_clickedC ? 1 : 0);
            if (n >= 3)
            {
                victoryPanel?.SetActive(true);
            }

        }

        private void OnDestroy()
        {
            if (btnA != null) btnA.onClick.RemoveListener(OnBtnAClick);
            if (btnB != null) btnB.onClick.RemoveListener(OnBtnBClick);
            if (btnC != null) btnC.onClick.RemoveListener(OnBtnCClick);
        }
    }
}