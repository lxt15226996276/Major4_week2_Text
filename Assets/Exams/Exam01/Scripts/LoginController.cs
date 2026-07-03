using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam01
{
    public class LoginController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private InputField accountInput;
        [SerializeField] private InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Text tipText;

        //账号 密码 场景名
        private const string CorrectAccount = "lixiaotong";
        private const string CorrectPassWord = "888888";

        private const string NextSceneName = "Exam01_ServerScene";

        void Start()
        {
            //按钮绑定登录事件
            loginButton.onClick.AddListener(OnLoginClick);

            accountInput.onSubmit.AddListener(_ => passwordInput.ActivateInputField());
            passwordInput.onSubmit.AddListener(_ => OnLoginClick());
            tipText.text = null;

        }


        /// <summary>
        /// 登录按钮点击时调用
        /// </summary>
        private void OnLoginClick()
        {
            string account = accountInput.text.Trim();
            string password = passwordInput.text.Trim();
            if (account == CorrectAccount && password == CorrectPassWord)
            {
                tipText.text = "登陆成功";
                Debug.Log("登陆成功");

                SceneManager.LoadScene(NextSceneName);
            }
            else
            {
                tipText.text = "账号或密码错误，请重试";
                accountInput.text = null;
                passwordInput.text = null;
                accountInput.ActivateInputField();
                Debug.Log("登录失败");
            }
        }
        
        /// <summary>
        /// 移除监听
        /// </summary>
        void OnDestroy()
        {
            loginButton.onClick.RemoveListener(OnLoginClick);
            accountInput.onSubmit.RemoveAllListeners();
            passwordInput.onSubmit.RemoveAllListeners();
        }

    }

}
