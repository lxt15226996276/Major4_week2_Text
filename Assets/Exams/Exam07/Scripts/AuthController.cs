using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam07
{
    public class AuthController : MonoBehaviour
    {
        [Header("Tree Panel")]
        [SerializeField] private GameObject intialPanel;
        [SerializeField] private GameObject loginlPanel;
        [SerializeField] private GameObject registerPanel;

        [Header("Initial Panel")]
        [SerializeField] private Button btnToLogin;
        [SerializeField] private Button btnToRegister;

        [Header("Register Panel ")]
        [SerializeField] private InputField accountRegInput;
        [SerializeField] private InputField passwordRegInput;
        [SerializeField] private Button btnRegisterSubmit;
        [SerializeField] private Button btnRegisterBack;

        [Header("Login Panel")]
        [SerializeField] private InputField accountLoginInput;
        [SerializeField] private InputField passwordLoginInput;
        [SerializeField] private Button btnLoginSubmit;
        [SerializeField] private Button btnLoginBack;
        [SerializeField] private Text tipText;
        private bool isLocked;
        private readonly string nextSceneName = "Exam07_Game";
        void Awake()
        {
            ShowInitalPanel();
            tipText.text = null;
        }

        void OnEnable()
        {
            btnToLogin.onClick.AddListener(ShowLoginlPanel);
            btnToRegister.onClick.AddListener(ShowRegisterPanel);
            btnLoginBack.onClick.AddListener(ShowInitalPanel);
            btnRegisterBack.onClick.AddListener(ShowInitalPanel);

            btnRegisterSubmit.onClick.AddListener(OnRegisterClick);
            btnLoginSubmit.onClick.AddListener(OnLoginClick);

        }

        void OnDestroy()
        {
            btnToLogin.onClick.RemoveListener(ShowLoginlPanel);
            btnToRegister.onClick.RemoveListener(ShowRegisterPanel);
            btnLoginBack.onClick.RemoveListener(ShowInitalPanel);
            btnRegisterBack.onClick.RemoveListener(ShowInitalPanel);

            btnRegisterSubmit.onClick.RemoveListener(OnRegisterClick);
            btnLoginSubmit.onClick.RemoveListener(OnLoginClick);
        }
        private void ShowInitalPanel() => ShowPanel(intialPanel);
        private void ShowLoginlPanel() => ShowPanel(loginlPanel);
        private void ShowRegisterPanel() => ShowPanel(registerPanel);

        /// <summary>
        /// 注册
        /// </summary>
        private void OnRegisterClick()
        {
            string account = accountRegInput.text;
            string password = passwordRegInput.text;

            if (AccountData.Instacne.Register(account, password, out string message))
            {
                accountRegInput.text = null;
                passwordRegInput.text = null;
                accountRegInput.ActivateInputField();
                ShowLoginlPanel();
                Debug.Log(message);
            }
            else
            {
                Debug.Log(message);
            }

        }
        /// <summary>
        /// 登录
        /// </summary>
        private void OnLoginClick()
        {
            if (isLocked) return;
            string account = accountLoginInput.text;
            string password = passwordLoginInput.text;
            if (AccountData.Instacne.TryLogin(account, password, out string message))
            {
                SceneManager.LoadScene(nextSceneName);
                Debug.Log(message);
                return;
            }

            tipText.text = message;
            StartCoroutine(StopTressSeconds());

            Debug.Log(message);

        }
        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="panel"></param>
        private void ShowPanel(GameObject panel)
        {
            intialPanel.SetActive(intialPanel == panel);
            loginlPanel.SetActive(loginlPanel == panel);
            registerPanel.SetActive(registerPanel == panel);
        }
        /// <summary>
        /// 暂停三秒
        /// </summary>
        /// <returns></returns>
        IEnumerator StopTressSeconds()
        {
            isLocked = true;
            btnLoginSubmit.interactable = false;
            accountLoginInput.text = null;
            passwordLoginInput.text = null;
            accountLoginInput.ActivateInputField();

            tipText.text = "登录失败 请等候3秒";
            yield return new WaitForSeconds(3);
            isLocked = false;
            btnLoginSubmit.interactable = true;
            tipText.text = null;

        }
    }

}
