using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exam.Exam04
{
    /// <summary>
    /// Exam04 登录注册：两面板互斥、Dictionary 校验、成功后进 Loading。
    /// </summary>
    public class AuthController : MonoBehaviour
    {
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;

        [Header("Login")]
        [SerializeField] private InputField loginAccountInput;
        [SerializeField] private InputField loginPasswordInput;
        [SerializeField] private Button btnRegister;
        [SerializeField] private Button btnLogin;
        [SerializeField] private Button btnExit;

        [Header("Register")]
        [SerializeField] private InputField registerAccountInput;
        [SerializeField] private InputField registerPasswordInput;
        [SerializeField] private Button btnConfirmRegister;
        [SerializeField] private Button btnBackRegister;

        private void Awake()
        {
            ShowPanel(loginPanel);
        }

        private void Start()
        {
            btnRegister.onClick.AddListener(OnOpenRegister);
            btnBackRegister.onClick.AddListener(OnBackToLogin);
            btnConfirmRegister.onClick.AddListener(OnRegisterClick);
            btnLogin.onClick.AddListener(OnLoginClick);
            btnExit.onClick.AddListener(OnExitClick);
        }

        /// <summary>思路：同一时刻只亮一个面板。</summary>
        private void ShowPanel(GameObject target)
        {
            loginPanel.SetActive(target == loginPanel);
            registerPanel.SetActive(target == registerPanel);
        }

        private void OnOpenRegister() => ShowPanel(registerPanel);
        private void OnBackToLogin() => ShowPanel(loginPanel);
        private void OnExitClick() => Application.Quit();

        private void OnRegisterClick()
        {
            string account = registerAccountInput.text.Trim();
            string password = registerPasswordInput.text.Trim();
            if (AccountData.Instance.Register(account, password, out string msg))
            {
                registerAccountInput.text = "";
                registerPasswordInput.text = "";
                ShowPanel(loginPanel);
            }
            Debug.Log(msg);
        }

        private void OnLoginClick()
        {
            string account = loginAccountInput.text.Trim();
            string password = loginPasswordInput.text.Trim();

            if (AccountData.Instance.TryLogin(account, password, out string msg))
            {
                SceneManager.LoadScene(SceneNames.Loading);
                return;
            }
            Debug.Log(msg);
            loginAccountInput.text = "";
            loginPasswordInput.text = "";
            loginAccountInput.ActivateInputField();
        }

        private void OnDestroy()
        {
            if (btnLogin != null) btnLogin.onClick.RemoveListener(OnLoginClick);
            if (btnRegister != null) btnRegister.onClick.RemoveListener(OnOpenRegister);
            if (btnConfirmRegister != null) btnConfirmRegister.onClick.RemoveListener(OnRegisterClick);
            if (btnBackRegister != null) btnBackRegister.onClick.RemoveListener(OnBackToLogin);
            if (btnExit != null) btnExit.onClick.RemoveListener(OnExitClick);
        }
    }
}