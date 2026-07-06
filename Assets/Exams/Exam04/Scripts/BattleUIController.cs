using UnityEngine;
using UnityEngine.UI;

namespace Exam.Exam04
{
    /// <summary>
    /// Exam04 战斗 UI：音乐/开火/关面板。
    /// [递进] mute 双反馈 · 开火 Attack Trigger · OnDestroy 解绑。
    /// </summary>
    public class BattleUIController : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject battlePanel;

        [Header("按钮")]
        [SerializeField] private Button btnMute;
        [SerializeField] private Button btnFire;
        [SerializeField] private Button btnClose;
        [SerializeField] private Text muteStateText;

        [Header("开火")]
        [SerializeField] private PlayerController playerController;

        [Header("BGM")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private Image BtnMuteImage;
        [SerializeField] private Sprite[] muteSprites;

        private bool _isMuted;


        private void Start()
        {
            btnMute.onClick.AddListener(ToggleMusic);
            btnFire.onClick.AddListener(OnFireClick);
            btnClose.onClick.AddListener(OnCloseClick);
            RefreshMuteText();
        }

        private void ToggleMusic()
        {
            _isMuted = !_isMuted;
            bgmSource.mute = _isMuted;
            RefreshMuteSpite();
            RefreshMuteText();
        }
        private void RefreshMuteSpite()
        {
            BtnMuteImage.sprite = _isMuted ? muteSprites[0] : muteSprites[1];
        }
        private void RefreshMuteText()
        {
            if (muteStateText != null)
                muteStateText.text = _isMuted ? "静音" : "开启";
        }

        private void OnFireClick()
        {
            playerController?.TryAttack();
        }

        private void OnCloseClick() => battlePanel.SetActive(false);

        private void OnDestroy()
        {
            if (btnMute != null) btnMute.onClick.RemoveListener(ToggleMusic);
            if (btnFire != null) btnFire.onClick.RemoveListener(OnFireClick);
            if (btnClose != null) btnClose.onClick.RemoveListener(OnCloseClick);
        }

    }
}