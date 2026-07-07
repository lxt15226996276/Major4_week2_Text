using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam09
{
    /// <summary>
    /// Exam09 登录：姓名全拼 + 8888 校验，成功进 Main 选关场景。
    /// [继承 §七] Exam03 LoginController · Trim + LoadScene。
    /// [递进] 密码 8888（非 6666）· 用 SceneNames.Main 常量。
    /// </summary>
    public class LoginController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private InputField accountInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Button loginButton;

        // [本卷固定账号] 姓名全拼 · 勿写死到 Input 里
        private const string CorrectAccount = "lixiaotong";
        private const string CorrectPassword = "8888";

        private void Start()
        {
            loginButton.onClick.AddListener(OnLoginClick);
        }

        /// <summary>
        /// 思路：Trim → 与 const 比较 → Log 卷面文案 → LoadScene(Main)。
        /// </summary>
        private void OnLoginClick()
        {
            string account = accountInput.text.Trim();
            string password = passwordInput.text.Trim();

            if (account == CorrectAccount && password == CorrectPassword)
            {
                Debug.Log("登陆成功");
                SceneManager.LoadScene(SceneNames.Main);
            }
            else
            {
                Debug.Log("登录失败");
            }
        }

        private void OnDestroy()
        {
            if (loginButton != null)
                loginButton.onClick.RemoveListener(OnLoginClick);
        }
    }
}