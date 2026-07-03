using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Exam.Exam04
{
    /// <summary>
    /// 登录场景：持久化保存用户名和密码
    /// </summary>
    public class LoginManager : MonoBehaviour
    {
        [Header("UI 引用")]
        [SerializeField] private InputField usernameInput;
        [SerializeField] private InputField passnameInput;
        [SerializeField] private Button saveButton;

        //PlayerPrefs的键名（持久化用的钥匙）
        private const string UsernameKey = "SaveUsername";
        private const string PasswordKey = "SavePassword";

        void Start()
        {
            //启动时自动读取上次保存的数据
            LoadData();
            //绑定按钮事件
            if (saveButton != null)
            {
                saveButton.onClick.AddListener(SaveData);
            }

        }

        void Update()
        {
            //正在输入账号或密码是，不响应空格跳场景
            if (usernameInput != null && usernameInput.isFocused) return;
            if (passnameInput != null && passnameInput.isFocused) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Exam04_BattleScene");
            }
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        void LoadData()
        {
            if (usernameInput != null)
            {
                usernameInput.text = PlayerPrefs.GetString(UsernameKey, "");
            }
            if (passnameInput != null)
            {
                passnameInput.text = PlayerPrefs.GetString(PasswordKey, "");
            }
            Debug.Log("读取成功：用户名 " + PlayerPrefs.GetString(UsernameKey, ""));
        }

        /// <summary>
        /// 把输入框里面的内容保存到PlaerPrefs
        /// </summary>
        void SaveData()
        {
            string username = usernameInput != null ? usernameInput.text : "";
            string password = passnameInput != null ? passnameInput.text : "";
            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.SetString(PasswordKey, password);
            PlayerPrefs.Save();
            Debug.Log("保存成功： 用户名：" + username);
        }
    }

}
