using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
# endif

namespace Exam.Exam04
{
    /// <summary>
    /// 登录场景UI ： 注册 登录 退出 面板切换
    /// </summary>
    public class LoginUIController : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;

        [Header("登录面板 - 输入与按钮")]
        [SerializeField] private InputField loginAccountInput;
        [SerializeField] private InputField loginPasswordInput;
        [SerializeField] private Button btnLogin;
        [SerializeField] private Button btnRegister;  // 打开注册
        [SerializeField] private Button btnExit;

        [Header("注册面板 - 输入与按钮")]
        [SerializeField] private InputField registerAccountInput;
        [SerializeField] private InputField registerPasswordInput;
        [SerializeField] private Button btnConfirmRegister; // RegisterButton「确定」
        [SerializeField] private Button btnBackRegister;     // 可选

        [Header("场景")]
        [SerializeField] private string createRoleSceneName = "Exam04_CreateRole";

        void Start()
        {
            //登录区
            btnRegister.onClick.AddListener(() => ShowPanel(registerPanel));
            btnLogin.onClick.AddListener(OnLoginClick);
            btnExit.onClick.AddListener(OnExitClick);

            loginAccountInput.onSubmit.AddListener(_ => loginPasswordInput.ActivateInputField());
            loginPasswordInput.onSubmit.AddListener(_ => OnLoginClick());

            //注册区
            btnConfirmRegister.onClick.AddListener(OnRegisterClick);
            btnBackRegister.onClick.AddListener(() => ShowPanel(loginPanel));
            ShowPanel(loginPanel);

        }
        /// <summary>
        /// 只显示Target，另一个关掉
        /// </summary>
        /// <param name="target"></param>
        private void ShowPanel(GameObject target)
        {
            loginPanel.SetActive(target == loginPanel);
            registerPanel.SetActive(target == registerPanel);
        }

        /// <summary>
        /// 注册确定：存字典->回登录界面
        /// </summary>
        private void OnRegisterClick()
        {
            string account = registerAccountInput.text.Trim();
            string password = registerPasswordInput.text.Trim();
            if (!AccountData.Instance.Register(account, password))
            {
                Debug.Log("注册失败：账号或密码不能为空");
                return;
            }

            //注册成功：清空输入框，返回登录界面
            registerAccountInput.text = null;
            registerPasswordInput.text = null;
            ShowPanel(loginPanel);
        }

        /// <summary>
        /// 登录验证,成功进入选择角色场景
        /// </summary>
        private void OnLoginClick()
        {
            string account = loginAccountInput.text.Trim();
            string password = loginPasswordInput.text.Trim();

            if (AccountData.Instance.TryLogin(account, password))
            {
                Debug.Log("登录成功");
                SceneManager.LoadScene(createRoleSceneName);
            }
            else
            {
                Debug.Log("登录失败");
                loginAccountInput.text = null;
                loginPasswordInput.text = null;
                loginAccountInput.ActivateInputField();
            }
        }
        /// <summary>
        /// 编辑器模式下 退出游戏 
        /// </summary>
        private void OnExitClick()
        {
            Debug.Log("退出");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;

#else 
            Application.Quit();
#endif

        }
    }
}

