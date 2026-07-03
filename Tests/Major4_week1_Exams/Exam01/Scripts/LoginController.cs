using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TerrainTools;
using UnityEngine.UI;
namespace Exam.Exam01
{
    public class LoginController : MonoBehaviour
    {
        [Header("ui 引用")]
        [SerializeField] private InputField accountInput;//账号输入框
        [SerializeField] private InputField passwordInput;//账号输入框
        [SerializeField] private Button loginButton;//登录按钮

        [Header("登录凭据")]
        [SerializeField] private string correctAccount = "lixiaotong";
        [SerializeField] private string correctPassword = "888888";

        [Header("场景设置")]
        [SerializeField] private string serveSceneName = "Exam01_Server";

        void Start()
        {
            //注册按钮事件
            loginButton.onClick.AddListener(OnLogoinButtonClick);
            //账号框回车，光标调到密码框
            accountInput.onSubmit.AddListener( _=> passwordInput.ActivateInputField());
            //密码回车，光标跳到密码框
            passwordInput.onSubmit.AddListener(_=> OnLogoinButtonClick());
        }

        /// <summary>
        /// 登录按钮回调
        /// </summary>
        void OnLogoinButtonClick()
        {
            //读取用户输入的字符串
            string inputAccout = accountInput.text;
            string inpuPassword = passwordInput.text;

            //同时登录成功跳转场景
            if (inputAccout == correctAccount && inpuPassword == correctPassword)
            {
                Debug.Log("登陆成功");

                SceneManager.LoadScene(serveSceneName);
            }
            else
            {
                Debug.Log("登录失败");
                accountInput.text = null;
                passwordInput.text = null;
                //光标自动回答账号框
                accountInput.ActivateInputField();
            }
        }
    }
}

