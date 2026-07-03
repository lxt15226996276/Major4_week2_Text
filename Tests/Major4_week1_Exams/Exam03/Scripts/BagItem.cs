using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam03
{
    /// <summary>
    /// 背包当个道具格子：显示图标，处理点击与选中高亮
    /// </summary>
    public class BagItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image bgImage;  // 格子底图（BgImage）
        [SerializeField] private Image iconImage;    // 道具图标（IconImage）
        [SerializeField] private Button button;      // 根节点 Button，可不拖，下面会 GetComponent

        private readonly Color normalColor = Color.white;
        private readonly Color selectedColor = Color.green;

        Action<BagItem> onClickBack;//点击通知BagController


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="onClick"></param>
        public void Init(Action<BagItem> onClick)
        {
            onClickBack = onClick;
            if (button == null) button.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            SetSelected(false);
        }

        private void OnButtonClick()
        {
            onClickBack?.Invoke(this);
        }
        /// <summary>
        /// 选中切换颜色
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (bgImage == null) return;
            bgImage.color = selected ? selectedColor : normalColor;
        }

    }
}

