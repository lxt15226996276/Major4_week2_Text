using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam03
{
    public class LoginController : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private InputField accountInput;//账号输入框
        [SerializeField] private InputField passwordInput;//账号输入框
        [SerializeField] private Button loginButton;

        [Header("登录凭据")]
        [SerializeField] private string correctAccount = "lixiaotong";  // 姓名全拼
        [SerializeField] private string correctPassword = "6666";     // 固定密码
        [Header("场景设置")]
        [SerializeField] private string mainSceneName = "Exam03_Main";

        void Start()
        {
            // 账号框按回车 → 焦点移到密码框（UX 小优化）
            accountInput.onSubmit.AddListener(_ => passwordInput.ActivateInputField());
            // 密码框按回车 → 直接尝试登录
            passwordInput.onSubmit.AddListener(_ => OnLoginButtonClick());
            loginButton.onClick.AddListener(OnLoginButtonClick);
        }
        /// <summary>
        /// 登录按钮核心逻辑：读取输入->比对 ->输出 
        /// </summary>
        void OnLoginButtonClick()
        {
            string inputAccout = accountInput.text.Trim();
            string inputPaasword = passwordInput.text.Trim();
            if (inputAccout == correctAccount && inputPaasword == correctPassword)
            {
                Debug.Log("登陆成功");
                SceneManager.LoadScene(mainSceneName);
            }
            else
            {
                Debug.Log("登录失败");
                // 失败后清空，方便重新输入
                accountInput.text = "";
                passwordInput.text = "";
                accountInput.ActivateInputField();  // 光标回到账号框
            }
        }
    }

}
