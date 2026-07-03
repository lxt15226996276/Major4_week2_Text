using System.Collections;
using UnityEngine.UI;
using UnityEngine;
namespace Exam.Exam03
{
    /// <summary>
    /// 背包总开关：开关控制面板 动态创建30个道具 管理单选
    /// </summary>
    public class BagController : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject bagPanel;       // BagPanel 整面板
        [SerializeField] private Button btnOpenBag;         // 右下角打开
        [SerializeField] private Button btnCloseBag;        // 右上角关闭

        [Header("动态生成")]
        [SerializeField] private Transform contentRoot;     // ScrollView/Viewport/Content
        [SerializeField] private GameObject bagItemPrefab;     // BagItem 预制体
        [SerializeField] private int itemCount = 30;        // 固定 30

        private BagItem currentSelected;                    // 当前选中的格子
        void Start()
        {
            //进场景先关背包
            bagPanel.SetActive(false);

            btnOpenBag.onClick.AddListener(() => bagPanel.SetActive(true));
            btnCloseBag.onClick.AddListener(() => bagPanel.SetActive(false));

            CreatItems();

        }
        /// <summary>
        /// 动态创建30个道具
        /// </summary>
        private void CreatItems()
        {
            for (int i = 0; i < itemCount; i++)
            {
                GameObject itemObj = Instantiate(bagItemPrefab, contentRoot, false);
                BagItem item = itemObj.GetComponent<BagItem>();
                if (item==null)
                {
                    continue;
                }
                item.Init(OnItemClicked);
            }
        }
        /// <summary>
        /// 某个格子被点击：互斥逻辑
        /// </summary>
        private void OnItemClicked(BagItem clicked)
        {
            if (currentSelected == clicked) return;
            currentSelected?.SetSelected(false);

            currentSelected = clicked;
            currentSelected.SetSelected(true);
            Debug.Log("颜色已经改变");

        }
    }
}

