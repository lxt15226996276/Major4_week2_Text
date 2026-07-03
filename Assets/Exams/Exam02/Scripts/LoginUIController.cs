using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam02
{
    public class LoginUIController : MonoBehaviour
    {
        [Header("UI 面板")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject initalPanel;


        [Header("Inital 面板")]
        [SerializeField] private Button loginPanelButton;
        [SerializeField] private Button registerPanelButton;

        [Header("Rigister 面板")]
        [SerializeField] private Button rigisterBackButton;
        [SerializeField] private Button registerDataButton;
        [SerializeField] private InputField accountRegisterInput;
        [SerializeField] private InputField passwordRegisterInput;

        [Header("Login 面板")]
        [SerializeField] private Button loginBackButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private InputField accountLoginInput;
        [SerializeField] private InputField passwordLoginInput;


        void Awake()
        {
            ShowPanel(initalPanel);
        }

        void Start()
        {
            //添加按钮注册事件
            loginPanelButton.onClick.AddListener(() => ShowPanel(loginPanel));
            registerPanelButton.onClick.AddListener(() => ShowPanel(registerPanel));
            rigisterBackButton.onClick.AddListener(() => ShowPanel(initalPanel));
            registerDataButton.onClick.AddListener(RegisterBtnOnClick);

            loginBackButton.onClick.AddListener(() => ShowPanel(initalPanel));
            loginButton.onClick.AddListener(LoginOnClick);


        }

        /// <summary>
        /// 三个面板互斥显示
        /// </summary>
        /// <param name="panel"></param>
        private void ShowPanel(GameObject panel)
        {
            loginPanel.SetActive(panel == loginPanel);
            registerPanel.SetActive(panel == registerPanel);
            initalPanel.SetActive(panel == initalPanel);
        }

        /// <summary>
        /// 注册账号 密码
        /// </summary>
        private void RegisterBtnOnClick()
        {
            string inpuAccount = accountRegisterInput.text;
            string inputPassword = passwordRegisterInput.text;
            string message = "";
            if (AccountData.Instance.Register(inpuAccount, inputPassword, out message))
            {
                accountRegisterInput.text = null;
                passwordRegisterInput.text = null;
                accountRegisterInput.ActivateInputField();
                ShowPanel(initalPanel);
            }

            Debug.Log(message);
        }

        /// <summary>
        /// 登录 
        /// </summary>
        private void LoginOnClick()
        {
            string inpuAccount = accountLoginInput.text;
            string inputPassword = passwordLoginInput.text;
            if (AccountData.Instance != null && AccountData.Instance.TryLogin(inpuAccount, inputPassword))
            {
                Debug.Log("登录成功，跳转场景");
                SceneManager.LoadScene("Exam02_ServerScene");
            }
            else
            {
                Debug.Log("登陆失败");
                accountLoginInput.text = null;
                passwordLoginInput.text = null;
                accountLoginInput.ActivateInputField();
            }
        }
        void OnDestroy()
        {
            if (loginPanelButton != null) loginPanelButton.onClick.RemoveAllListeners();
            if (registerPanelButton != null) registerPanelButton.onClick.RemoveAllListeners();
            if (rigisterBackButton != null) rigisterBackButton.onClick.RemoveAllListeners();
            if (registerDataButton != null) registerDataButton.onClick.RemoveAllListeners();
            if (loginBackButton != null) loginBackButton.onClick.RemoveAllListeners();
            if (loginButton != null) loginButton.onClick.RemoveAllListeners();
        }
    }

}
