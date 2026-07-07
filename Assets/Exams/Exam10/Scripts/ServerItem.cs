using System;
using UnityEngine;
using UnityEngine.UI;

namespace Exam.Exam10
{
    /// <summary>
    /// 选服列表单项：显示服名 · 点击回调给 ServerController。
    /// [继承 Exam08 ServerItem]
    /// </summary>
    public class ServerItem : MonoBehaviour
    {
        [SerializeField] private Button itemBtn;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Sprite[] selectedSprites;

        private Action<ServerItem> _onSelected;

        /// <summary>由 ServerController 生成后调用，传入服名和点击回调。</summary>
        public void Init(string serverName, Action<ServerItem> onSelected)
        {
            SetSelected(false);
            itemNameText.text = serverName;
            _onSelected = onSelected;
            itemBtn.onClick.AddListener(OnBtnClick);
        }

        public void SetSelected(bool selected)
        {
            if (selectedSprites == null || selectedSprites.Length < 2) return;
            GetComponent<Image>().sprite = selected ? selectedSprites[0] : selectedSprites[1];
        }

        public string GetItemName() => itemNameText.text;

        private void OnBtnClick() => _onSelected?.Invoke(this);

        private void OnDestroy()
        {
            if (itemBtn != null)
                itemBtn.onClick.RemoveListener(OnBtnClick);
        }
    }
}