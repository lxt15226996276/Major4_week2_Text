using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam10
{
    /// <summary>
    /// Exam10 登录：账号 + 密码 + 姓名 均不能为空，成功后进 Loading。
    /// [递进 vs Exam09] 无固定 8888 · 三字段判空 · [递进 vs Exam08] namespace Exam.Exam10。
    /// </summary>
    public class LoginController : MonoBehaviour
    {
        [Header("三个输入框")]
        [SerializeField] private InputField accountInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private InputField nameInput;

        [Header("按钮与提示")]
        [SerializeField] private Button loginBtn;
        [SerializeField] private Text tipText;

        private void Start()
        {
            loginBtn.onClick.AddListener(OnLoginClick);
            if (tipText != null)
                tipText.text = string.Empty;
        }

        /// <summary>
        /// 思路：Trim 三个 Input → 判空 → 写入 GameSession → NextScene=Server → LoadScene(Loading)。
        /// </summary>
        private void OnLoginClick()
        {
            string account = accountInput.text.Trim();
            string password = passwordInput.text.Trim();
            string name = nameInput.text.Trim();

            if (string.IsNullOrEmpty(account)
                || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(name))
            {
                if (tipText != null)
                    tipText.text = "账号 密码 姓名 不能为空";
                return;
            }

            // 存起来，后面 Server / Main 场景还能用
            GameSession.Account = account;
            GameSession.Password = password;
            GameSession.PlayerName = name;

            // 告诉 Loading：加载完去选服场景
            GameSession.NextScene = SceneNames.Server;

            SceneManager.LoadScene(SceneNames.Loading);
        }

        private void OnDestroy()
        {
            if (loginBtn != null)
                loginBtn.onClick.RemoveListener(OnLoginClick);
        }
    }
}