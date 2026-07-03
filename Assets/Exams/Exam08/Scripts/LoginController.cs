using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam08
{
    public class LoginController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private InputField accountInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private InputField nameInput;
        [SerializeField] private Button loginBtn;
        [SerializeField] private Text tipText;

        private const string LoadingSceneName = "Exam08_LoadingScene";
        private const string AfterSelectScene = "Exam08_ServerScene";

        void Start()
        {
            loginBtn.onClick.AddListener(OnLoginBtnClick);
            tipText.text = null;
        }

        private void OnLoginBtnClick()
        {
            string account = accountInput.text.Trim();
            string password = passwordInput.text.Trim();
            string name = nameInput.text.Trim();
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                tipText.text = "账号 密码 用户名 不能为空";
                return;
            }

            GameSession.Account = account;
            GameSession.Password = password;
            GameSession.PlayerName = name;
            GameSession.NextScene = AfterSelectScene;

            SceneManager.LoadScene(LoadingSceneName);
        }
        /// <summary>
        /// 场景卸载前解绑，避免重复 AddListener。
        /// </summary>
        void OnDestroy()
        {
            if (loginBtn != null)
                loginBtn.onClick.RemoveListener(OnLoginBtnClick);

        }

    }
}

