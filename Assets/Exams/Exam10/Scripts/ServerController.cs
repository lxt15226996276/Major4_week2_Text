using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam10
{
    /// <summary>
    /// Exam10 选服：动态生成 5 个 ServerItem · 点击后进 Loading → Main。
    /// [递进 vs Exam08] serverCount=5 · 用 SceneNames 常量。
    /// </summary>
    public class ServerController : MonoBehaviour
    {
        [Header("列表")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform contentRoot;

        [Header("选中提示")]
        [SerializeField] private Text selectedText;

        // Exam10：5 个不同服名（卷面要求 5 服）
        private readonly string[] _serverNames =
        {
            "一区 星火",
            "二区 星耀",
            "三区 星云",
            "四区 星辰",
            "五区 星汉"
        };

        private ServerItem _currentItem;

        private void Awake()
        {
            if (selectedText != null)
                selectedText.text = string.Empty;
        }

        private void Start()
        {
            CreateServerItems();
        }

        /// <summary>思路：循环 5 次 Instantiate itemPrefab → Init 服名 → 绑点击。</summary>
        private void CreateServerItems()
        {
            for (int i = 0; i < _serverNames.Length; i++)
            {
                var obj = Instantiate(itemPrefab, contentRoot);
                var item = obj.GetComponent<ServerItem>();
                string name = _serverNames[i];
                item.Init(name, OnServerItemClick);
            }
        }

        /// <summary>思路：高亮选中项 → 记 SelectedServer → NextScene=Main → Loading。</summary>
        private void OnServerItemClick(ServerItem item)
        {
            _currentItem?.SetSelected(false);
            _currentItem = item;
            _currentItem.SetSelected(true);

            if (selectedText != null)
                selectedText.text = item.GetItemName();

            GameSession.SelectedServer = item.GetItemName();
            GameSession.NextScene = SceneNames.Main;

            SceneManager.LoadScene(SceneNames.Loading);
        }
    }
}