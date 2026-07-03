using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Text_Login : MonoBehaviour
{
    [SerializeField] private string correctAccount = "lixiaotong";
    [SerializeField] private string correctPassword = "888888";
    [SerializeField] private InputField accountInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private string serveSceneName = "Exam01_Server";

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        accountInput.onSubmit.AddListener(_ => passwordInput.ActivateInputField());
        passwordInput.onSubmit.AddListener(_ => OnLoginButtonClick());
    }

    private void OnLoginButtonClick()
    {
        string inputAccout = accountInput.text;
        string inputPassword = passwordInput.text;
        if (inputAccout == correctAccount && inputPassword == correctPassword)
        {
            Debug.Log("登录成功");
            SceneManager.LoadScene(serveSceneName);
        }
        else
        {
            Debug.Log("登录失败");
            accountInput.text = null;
            passwordInput.text = null;
        }

    }


}
