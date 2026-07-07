using UnityEngine;
using UnityEngine.UI;

namespace Exam.Exam10
{
    /// <summary>
    /// Exam10 玩家血量：InvokeRepeating 每秒 -10 · 加血 +50 · 血尽开 VictoryPanel。
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private GameObject victoryPanel;

        private const int MaxHp = 100;
        private const int AutoDamagePerSec = 10;
        private const int HealAmount = 50;
        private const float DamageInterval = 1f;   // 每 1 秒扣一次

        private int _hp;
        private bool _isVictory;

        private void Start()
        {
            _hp = MaxHp;

            if (hpSlider != null)
            {
                hpSlider.maxValue = MaxHp;
                hpSlider.value = _hp;
            }

            if (victoryPanel != null)
                victoryPanel.SetActive(false);

            // 思路：1 秒后开始，之后每 1 秒调用 ApplyAutoDamage
            InvokeRepeating(nameof(ApplyAutoDamage), DamageInterval, DamageInterval);
        }

        /// <summary>InvokeRepeating 回调 · 每秒自动扣 10。</summary>
        private void ApplyAutoDamage()
        {
            if (_isVictory) return;

            ApplyDamage(AutoDamagePerSec);
        }

        /// <summary>加血按钮调用 · 一次 +50。</summary>
        public void Heal()
        {
            if (_isVictory) return;

            _hp = Mathf.Clamp(_hp + HealAmount, 0, MaxHp);
            SyncSlider();
        }

        /// <summary>统一扣血 + 同步 Slider + 判胜利。</summary>
        private void ApplyDamage(int amount)
        {
            _hp = Mathf.Clamp(_hp - amount, 0, MaxHp);
            SyncSlider();

            if (_hp <= 0)
            {
                _isVictory = true;
                if (victoryPanel != null)
                    victoryPanel.SetActive(true);

                // 胜利后停止自动扣血
                CancelInvoke(nameof(ApplyAutoDamage));
            }
        }

        private void SyncSlider()
        {
            if (hpSlider != null)
                hpSlider.value = _hp;
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(ApplyAutoDamage));
        }
    }
}