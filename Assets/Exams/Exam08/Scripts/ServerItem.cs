using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
namespace Exam.Exam08
{
    public class ServerItem : MonoBehaviour
    {

        [SerializeField] private Button itemBtn;
        [SerializeField] private Text itemNameText;
        [SerializeField] private Sprite[] selectedSprites;
        Action<ServerItem> onSelectedCallBack;
        private bool isSelected;

        public void Init(string name, Action<ServerItem> onSelected)
        {
            SetSelected(false);
            itemNameText.text = name;
            onSelectedCallBack = onSelected;
            itemBtn.onClick.AddListener(OnBtnClik);
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            GetComponent<Image>().sprite = isSelected ? selectedSprites[0] : selectedSprites[1];
        }

        private void OnBtnClik()
        {
            onSelectedCallBack?.Invoke(this);
        }
        public string GetItemName() => itemNameText.text;
        void OnDestroy()
        {
            itemBtn.onClick.RemoveListener(OnBtnClik);
        }
    }
}

