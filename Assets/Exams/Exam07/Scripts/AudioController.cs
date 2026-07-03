using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam07
{
    public class AudioController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Button btnMute;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Sprite[] musicSprites;
        private bool isMute;
        void Start()
        {
            isMute = audioSource.mute;
            btnMute.onClick.AddListener(OnMuteBtnClick);

        }

        /// <summary>
        /// 静音
        /// </summary>
        private void OnMuteBtnClick()
        {
            audioSource.mute = !audioSource.mute;
            isMute = audioSource.mute;
            ReFreshUI();
        }

        /// <summary>
        /// 跟新UI 显示
        /// </summary>
        private void ReFreshUI()
        {
            btnMute.image.sprite = isMute ? musicSprites[0] : musicSprites[1];
            btnMute.GetComponentInChildren<Text>().text = isMute ? "静音" : "开启";
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        void OnDestroy()
        {
            if (btnMute != null) btnMute.onClick.RemoveListener(OnMuteBtnClick);
        }
    }
}

