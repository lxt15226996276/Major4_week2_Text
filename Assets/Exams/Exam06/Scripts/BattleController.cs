using UnityEngine;
using UnityEngine.UI;

namespace Exam.Exam06
{
    /// <summary>
    /// Exam06 战斗：三钮扣血 20/30/50，hp≤0 显示 GameOverPanel 并 Log。
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        // --- UI 引用 ---
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Text hpLabel;
        [SerializeField] private GameObject gameOverPanel;

        // --- 攻击钮 · 命名方法绑定 ---
        [SerializeField] private Button btnAttack20;
        [SerializeField] private Button btnAttack30;
        [SerializeField] private Button btnAttack50;

        // --- 战斗参数 ---
        [SerializeField] private int maxHp = 100;
        private int currentHp;
        /// <summary>结束后忽略扣血输入。</summary>
        private bool isGameOver;

        /// <summary>
        /// 思路：初始化 HP/Slider → 隐藏 Panel → 绑三钮命名回调。
        /// </summary>
        private void Start()
        {
            currentHp = maxHp;
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
            gameOverPanel.SetActive(false);
            RefreshHpText();

            btnAttack20.onClick.AddListener(OnAttack20);
            btnAttack30.onClick.AddListener(OnAttack30);
            btnAttack50.onClick.AddListener(OnAttack50);
        }

        private void OnAttack20() => TakeDamage(20);
        private void OnAttack30() => TakeDamage(30);
        private void OnAttack50() => TakeDamage(50); // 第三钮 50 · 非 Exam05 的 40

        /// <summary>
        /// 思路：门闩 → Clamp 扣血 → 刷 UI → hp≤0 进入结束态。
        /// </summary>
        private void TakeDamage(int damage)
        {
            if (isGameOver) return;

            currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);
            hpSlider.value = currentHp; // §11.20：只 value · 不 Hide Fill
            RefreshHpText();

            if (currentHp <= 0)
            {
                isGameOver = true;
                gameOverPanel.SetActive(true);
                Debug.Log("游戏结束");
            }
        }

        /// <summary>思路：同步 HP 文字（Label 可选）。</summary>
        private void RefreshHpText()
        {
            if (hpLabel != null)
                hpLabel.text = "生命值：" + currentHp;
        }

        /// <summary>思路：对称解绑三钮，防场景卸载后委托残留。</summary>
        private void OnDestroy()
        {
            if (btnAttack20 != null) btnAttack20.onClick.RemoveListener(OnAttack20);
            if (btnAttack30 != null) btnAttack30.onClick.RemoveListener(OnAttack30);
            if (btnAttack50 != null) btnAttack50.onClick.RemoveListener(OnAttack50);
        }
    }
}