using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Controller : MonoBehaviour
{
    [SerializeField] private GameObject serverItmePrefab;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private int serverItemeCount = 10;
    [SerializeField] private Text serverNameText;
    [SerializeField] private GameObject panel;
    private Text_Item currentItem;
    [SerializeField] private Button closeButton;

    void Start()
    {
        CreatServerList();
        closeButton.onClick.AddListener(OnCloseClick);
    }
    /// <summary>
    /// 创建服务器列表
    /// </summary>
    private void CreatServerList()
    {
        for (int i = 1; i <= serverItemeCount; i++)
        {
            GameObject serverObj = Instantiate(serverItmePrefab, contentRoot);
            Text_Item item = serverObj.GetComponent<Text_Item>();
            string sereverName = i + "区 服务器";
            item.Init(sereverName, OnClickSelected);
        }
    }


    /// <summary>
    /// 鼠标点击执行互斥 同步顶部文本
    /// </summary>
    /// <param name="item"></param>
    private void OnClickSelected(Text_Item item)
    {

        currentItem?.SetSelected(false);
        currentItem = item;
        currentItem.SetSelected(true);
        serverNameText.text = currentItem.GetCurrentName();
    }

    private void OnCloseClick()
    {
        panel.SetActive(false);
    }
}
