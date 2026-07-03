using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam01
{
    public class ServerLsitController : MonoBehaviour
    {
        [Header("UI资源引用")]
        [SerializeField] private Transform contentRoot;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private GameObject selectedItem;
        [SerializeField] private Text selectedItemText;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject serverPanel;
        //条目数量
        [SerializeField] private int itemCount;

        private ServerItem currentSelected;

        void Awake()
        {
            selectedItem.SetActive(false);
        }

        void Start()
        {
            CreatServerItems();
            closeButton.onClick.AddListener(() => serverPanel.SetActive(false));
        }
        /// <summary>
        /// 动态创建十个条目
        /// </summary>
        private void CreatServerItems()
        {
            for (int i = 1; i <= itemCount; i++)
            {
                string itemName = i + " 区服务器";
                var obj = Instantiate(itemPrefab, contentRoot);
                var item = obj.GetComponent<ServerItem>();
                item.Init(itemName, SelectedItem);

                item.SetSelected(false);
            }
        }

        /// <summary>
        /// 条目被选中执行的逻辑
        /// </summary>
        /// <param name="item"></param>
        private void SelectedItem(ServerItem item)
        {
            currentSelected?.SetSelected(false);
            currentSelected = item;
            currentSelected.SetSelected(true);
            selectedItem.SetActive(true);
            selectedItemText.text = item.GetItemName();
        }

        /// <summary>
        /// 移除监听事件
        /// </summary>
        void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}

