using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Item : MonoBehaviour
{
    [SerializeField] private Text serverNameText;
    Action<Text_Item> onClickSelected;
    [SerializeField] private Button serverItemButton;
    [SerializeField] private Image serverItemImage;
    private readonly Color normalColor = Color.white;
    private readonly Color selectedColor = new Color(1.0f, 0.85f, 0.4f);


    void Start()
    {
        serverItemButton.onClick.AddListener(OnItemClick);
    }

    /// <summary>
    /// 初始化条目
    /// </summary>
    public void Init(string serverNmae, Action<Text_Item> onCallback)
    {
        serverNameText.text = serverNmae;
        onClickSelected = onCallback;
        //设置初始颜色状态
        SetSelected(false);
    }

    /// <summary>
    /// 鼠标点中回调
    /// </summary>
    private void OnItemClick()
    {
        onClickSelected?.Invoke(this);
    }
    /// <summary>
    /// 设置取消/选中状态
    /// </summary>
    public void SetSelected(bool isSelected)
    {
        serverItemImage.color = isSelected ? selectedColor : normalColor;
    }

    public string GetCurrentName()
    {
        return serverNameText.text;
    }

}
