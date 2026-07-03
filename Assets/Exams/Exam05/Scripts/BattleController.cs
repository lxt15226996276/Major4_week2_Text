using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam05
{
    public class BattleController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Button buttonAttack20;
        [SerializeField] private Button buttonAttack30;
        [SerializeField] private Button buttonAttack40;
        [SerializeField] private Button buttonAttack50;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Text hpLabelText;
        [SerializeField] private GameObject gameOverPanel;
        private int maxHp;
        private int currentHp;
        private bool isGameOver;
        void Awake()
        {
            maxHp = 100;
            currentHp = maxHp;
            hpSlider.value = currentHp;
            RefreshHpText();
        }


        private void OnAttack20() => TakeDamage(20);
        private void OnAttack30() => TakeDamage(30);
        private void OnAttack40() => TakeDamage(40);
        private void OnAttack50() => TakeDamage(50);
        /// <summary>
        /// 按钮血量减少
        /// </summary>
        /// <param name="damage"></param>
        private void TakeDamage(int damage)
        {
            if (isGameOver) return;

            currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);

            RefreshHpUI();

            if (currentHp <= 0)
            {
                EnterGameOver();

            }
        }

        /// <summary>
        /// 设置介绍状态： 按钮禁用 显示结束面板
        /// </summary>
        private void EnterGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            SetAttackButtonInteractable(false);
            gameOverPanel.SetActive(true);
            Debug.Log("游戏结束");

        }
        /// <summary>
        /// 批量设置四个按钮是否可点
        /// </summary>
        /// <param name="interactable"></param>
        private void SetAttackButtonInteractable(bool interactable)
        {
            if (buttonAttack20 != null) buttonAttack20.interactable = interactable;
            if (buttonAttack30 != null) buttonAttack30.interactable = interactable;
            if (buttonAttack40 != null) buttonAttack40.interactable = interactable;
            if (buttonAttack50 != null) buttonAttack50.interactable = interactable;
        }
        /// <summary>
        /// 跟新UI
        /// </summary>
        private void RefreshHpUI()
        {
            hpSlider.value = currentHp;
            RefreshHpText();
        }
        /// <summary>
        /// 更新血量条显示文本
        /// </summary>
        private void RefreshHpText()
        {
            if (hpLabelText != null)
                hpLabelText.text = "生命值  " + currentHp;
        }
        /// <summary>
        /// 启用绑定按钮
        /// </summary>
        private void OnEnable()
        {
            if (buttonAttack20 != null) buttonAttack20.onClick.AddListener(OnAttack20);
            if (buttonAttack30 != null) buttonAttack30.onClick.AddListener(OnAttack30);
            if (buttonAttack40 != null) buttonAttack40.onClick.AddListener(OnAttack40);
            if (buttonAttack50 != null) buttonAttack50.onClick.AddListener(OnAttack50);
        }
        /// <summary>
        /// 移除事件监听
        /// </summary>
        void OnDestroy()
        {
            if (buttonAttack20 != null) buttonAttack20.onClick.RemoveListener(OnAttack20);
            if (buttonAttack30 != null) buttonAttack30.onClick.RemoveListener(OnAttack30);
            if (buttonAttack40 != null) buttonAttack40.onClick.RemoveListener(OnAttack40);
            if (buttonAttack50 != null) buttonAttack50.onClick.RemoveListener(OnAttack50);
        }
    }
}

