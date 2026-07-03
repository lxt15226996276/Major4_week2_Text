using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam08
{
    public class ServerController : MonoBehaviour
    {
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform contenrRoot;
        [SerializeField] private Text selectedText;
        private int serverCount = 3;
        private ServerItem currentItem;
        private string[] itemNames = { "一区 星火", "二区 星耀", "三区 星云" };

        private const string AfterSelectScene = "Exam08_MainScene";
        void Awake()
        {
            selectedText.text = null;
        }
        private void Start()
        {
            CreatServerItem();
        }
        private void CreatServerItem()
        {
            for (int i = 0; i < serverCount; i++)
            {
                var obj = Instantiate(itemPrefab, contenrRoot);
                var item = obj.GetComponent<ServerItem>();
                string name = itemNames[i];
                item.Init(name, OnServerItemClick);
            }
        }

        private void OnServerItemClick(ServerItem item)
        {

            currentItem?.SetSelected(false);
            currentItem = item;
            currentItem.SetSelected(true);
            selectedText.text = item.GetItemName();
            GameSession.NextScene = AfterSelectScene; ;
            SceneManager.LoadScene("Exam08_LoadingScene");

            Debug.Log("进入下一个场景");
        }
    }
}

