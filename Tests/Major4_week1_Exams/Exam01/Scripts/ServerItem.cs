using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam01
{
    /// <summary>
    /// 单台服务器显示ui:显示区域名，处理点击选中
    /// </summary>
    public class ServerItem : MonoBehaviour
    {

        [SerializeField] private Text serverNameText;//区服名文本
        [SerializeField] private Image backgroundImage;//条目底图，用于高亮；

        private Button button;
        private Action<ServerItem> onClickCallback;//点击回调，通知列表控制器
        //未选中/选中时的底图颜色
        private readonly Color normalColor = Color.white;
        private readonly Color selectedColor = new Color(1f, 0.85f, 0.4f);//淡黄色高亮

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnItemClick);
        }
        /// <summary>
        /// 初始化条目数据,由ServerListController在Instantiate后调用
        /// </summary>
        public void SetUp(string serverName, Action<ServerItem> callback)
        {
            serverNameText.text = serverName;
            onClickCallback = callback;
            SetSelected(false);
        }
        /// <summary>
        /// 设置选中/取消选中状态
        /// </summary>
        /// <param name="selected"></param>
        public void SetSelected(bool selected)
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }
        /// <summary>
        /// 按钮点击：通知列表控制器处理互斥选中
        /// </summary>
        private void OnItemClick()
        {
            onClickCallback?.Invoke(this);
        }
        /// <summary>
        /// 获得当前区服名
        /// </summary>
        public string GetServerName()
        {
            return serverNameText.text;
        }
    }

}
