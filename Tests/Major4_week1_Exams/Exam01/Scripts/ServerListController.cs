using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam01
{
    /// <summary>
    /// 选服列表控制器：动态创建10条 选中互斥 关闭面板
    /// </summary>
    public class ServerListController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private Transform contentRoot;//Scrollview/Content
        [SerializeField] private GameObject serverItemPrefab;//服务器条目预制体
        [SerializeField] private GameObject serverPanel;//Scrollview/Content
        [SerializeField] private Button closeButton;//Scrollview/Content 
        [SerializeField] private Text selectedServerText;//Scrollview/Content

        [Header("服务器数据")]
        [SerializeField] private int serverCount = 10;
        private ServerItem currentSelected;

        void Start()
        {
            CreatServerList();
            closeButton.onClick.AddListener(OnCloseClick);
        }
        /// <summary>
        /// 动态创建服务器列表
        /// </summary>
        private void CreatServerList()
        {
            for (int i = 1; i <= serverCount; i++)
            {
                //在ContentRoot 下克隆一条ServerItem
                GameObject itemObj = Instantiate(serverItemPrefab, contentRoot);
                //拿到克隆体上的ServerItem脚本
                ServerItem item = itemObj.GetComponent<ServerItem>();
                //组装区服名
                string serverName = i + "区 服务器";

                //初始化文本+绑定点击回调
                item.SetUp(serverName, OnServerItemClick);
            }
        }
        /// <summary>
        /// 某服务器被点击：互斥选中 同步顶部
        /// </summary>
        private void OnServerItemClick(ServerItem clickedItem)
        {
            //取消上一条的选择状态
            currentSelected?.SetSelected(false);

            //选中当前这条
            currentSelected = clickedItem;
            currentSelected.SetSelected(true);

            //同步顶部文字
            selectedServerText.text = clickedItem.GetServerName();
        }
        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnCloseClick()
        {
            serverPanel.SetActive(false);
        }
    }
}

