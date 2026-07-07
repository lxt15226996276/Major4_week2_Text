using UnityEngine;
using UnityEngine.UI;

namespace Exam.Exam10
{
    /// <summary>
    /// Exam10 主场景 UI：BtnMute 一键静音 · Heart 调 PlayerHealth.Heal()。
    /// [继承 Exam04] mute 思路：bgmSource.mute 取反。
    /// </summary>
    public class MainUIController : MonoBehaviour
    {
        [SerializeField] private Button btnMusic;
        [SerializeField] private Button btnHeal;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private PlayerHealth playerHealth;

        private bool _isMuted;

        private void Start()
        {
            if (btnMusic != null) btnMusic.onClick.AddListener(ToggleMusic);
            if (btnHeal != null) btnHeal.onClick.AddListener(OnHealClick);
        }

        /// <summary>思路：bool 取反 → AudioSource.mute。</summary>
        private void ToggleMusic()
        {
            if (bgmSource == null) return;

            _isMuted = !_isMuted;
            bgmSource.mute = _isMuted;
        }

        private void OnHealClick()
        {
            playerHealth?.Heal();
        }

        private void OnDestroy()
        {
            if (btnMusic != null) btnMusic.onClick.RemoveListener(ToggleMusic);
            if (btnHeal != null) btnHeal.onClick.RemoveListener(OnHealClick);
        }
    }
}