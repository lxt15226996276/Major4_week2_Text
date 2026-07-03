using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam02
{

    public class MainUIController : MonoBehaviour
    {

        [Header("UI 面板")]
        [SerializeField] private GameObject initialPanel;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;

        [Header("初始界面按钮")]
        [SerializeField] private Button btnLogin;
        [SerializeField] private Button btnRegister;

        [Header("返回按钮")]
        [SerializeField] private Button btnBackLogin;
        [SerializeField] private Button btnBackRegister;

        [Header("注册界面")]
        [SerializeField] private InputField registerAccountInput;
        [SerializeField] private InputField registerPasswordInput;
        [SerializeField] private Button btnDoRegister;

        [Header("登录界面")]
        [SerializeField] private InputField loginAccountInput;
        [SerializeField] private InputField loginPasswordInput;
        [SerializeField] private Button btnDoLogin;
        [Header("场景名")]
        [SerializeField] private string serverSceneName = "Exam02_Server";

        void Start()
        {
            //点击之后切换面板
            btnLogin.onClick.AddListener(() => ShowPanel(loginPanel));
            btnRegister.onClick.AddListener(() => ShowPanel(registerPanel));

            btnBackLogin.onClick.AddListener(() => ShowPanel(initialPanel));
            btnBackRegister.onClick.AddListener(() => ShowPanel(initialPanel));

            btnDoRegister.onClick.AddListener(OnRegisterClick);
            btnDoLogin.onClick.AddListener(OnLoginClik);

            loginAccountInput.onSubmit.AddListener(_ => loginPasswordInput.ActivateInputField());
            ShowPanel(initialPanel);
        }

        /// <summary>
        /// 只显示目标面板，其他两个关掉
        /// </summary>
        private void ShowPanel(GameObject target)
        {
            initialPanel.SetActive(target == initialPanel);
            loginPanel.SetActive(target == loginPanel);
            registerPanel.SetActive(target == registerPanel);
        }

        /// <summary>
        /// 点击注册： 写入字典，回归初始界面
        /// </summary>
        public void OnRegisterClick()
        {
            string acccount = registerAccountInput.text.Trim();
            string password = registerPasswordInput.text.Trim();
            if (!AccountData.Instance.Register(acccount, password))
            {
                Debug.Log("注册失败：账号或密码不能为空");
                return;
            }

            registerAccountInput.text = "";
            registerPasswordInput.text = "";
            registerAccountInput.ActivateInputField();
            ShowPanel(initialPanel);
        }
        /// <summary>
        /// 点击登录，字典比对
        /// </summary>
        public void OnLoginClik()
        {
            string acccount = loginAccountInput.text.Trim();
            string password = loginPasswordInput.text.Trim();
            if (AccountData.Instance.TryLogin(acccount, password))
            {
                Debug.Log("登录成功");
                SceneManager.LoadScene(serverSceneName);
            }
            else
            {
                Debug.Log("登陆失败");   // 试卷失败文案（注意是「登陆」）
            }
            loginAccountInput.text = "";
            loginPasswordInput.text = "";
            loginAccountInput.ActivateInputField();
        }
    }

}
