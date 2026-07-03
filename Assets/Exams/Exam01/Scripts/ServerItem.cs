using System;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam01
{
    public class ServerItem : MonoBehaviour
    {
        [SerializeField] private Text serverItemNameText;
        [SerializeField] private Button itemButton;
        Action<ServerItem> onSelecedCallBack;

        [SerializeField] private Image serverItemImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite highlightedSprite;
        private bool isSelected;


        /// <summary>
        /// 条目初始化 服务器命名
        /// </summary>
        public void Init(string itemNmae, Action<ServerItem> onSelected)
        {
            serverItemNameText.text = itemNmae;
            onSelecedCallBack = onSelected;

            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(OnItemClick);
        }

        private void OnItemClick()
        {
            onSelecedCallBack?.Invoke(this);
            // Invoke「调用、执行」委托内部保存的那个方法
        }

        /// <summary>
        /// 设置条目的选中状态
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            serverItemImage.sprite = selected ? highlightedSprite : normalSprite;
        }

        /// <summary>
        /// 返回一个名字
        /// </summary>
        public string GetItemName()
        {
            return serverItemNameText.text;
        }
    }
}

