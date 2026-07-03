using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam03
{
    /// <summary>
    /// Exam03 登录：全拼 + 6666 校验，成功进 GameScene。
    /// </summary>
    public class LoginController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private InputField accountInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Button loginButton;

        private readonly string CorrectAccount = "lixiaotong";
        private readonly string CorrectPassword = "6666";
        private readonly string GameSceneName = "Exam03_GameScene";


        void Start()
        {
            loginButton.onClick.AddListener(OnLoginClick);

        }

        /// <summary>
        /// 思路：Trim → 与 const 比较 → Log 卷面文案 → LoadScene(Game)。
        /// </summary>
        private void OnLoginClick()
        {
            string account = accountInput.text.Trim();
            string password = passwordInput.text.Trim();

            if (account == CorrectAccount && password == CorrectPassword)
            {
                Debug.Log("登陆成功");

                SceneManager.LoadScene(GameSceneName);
            }
            else
            {
                Debug.Log("登录失败");

            }
        }

        void OnDestroy()
        {
            if (loginButton != null)
                loginButton.onClick.RemoveListener(OnLoginClick);
        }
    }
}