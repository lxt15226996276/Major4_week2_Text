using System.Collections.Generic;
using UnityEngine;

namespace Exam.Exam07
{
    public class AccountData : MonoBehaviour
    {
        //单利
        public static AccountData Instacne;
        public string CurrentUserName { get; private set; }
        private Dictionary<string, string> accoutDic = new Dictionary<string, string>();
        void Awake()
        {
            if (Instacne! != null && Instacne != this)
            {
                Destroy(gameObject);
                return;
            }
            Instacne = this;

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public bool Register(string account, string password, out string message)
        {
            string accoutInput = account.Trim();
            string passwordInput = password.Trim();

            if (string.IsNullOrEmpty(accoutInput) || string.IsNullOrEmpty(passwordInput))
            {
                message = "账号或密码不能为空";
                return false;
            }
            if (accoutDic.ContainsKey(accoutInput))
            {
                message = "账号已存在";
                return false;
            }

            accoutDic.Add(accoutInput, passwordInput);
            message = $"注册成功 账号：{accoutInput}  密码：{passwordInput}";
            return true;
        }
        /// <summary>
        /// 尝试登录
        /// </summary>
        /// <returns></returns>
        public bool TryLogin(string account, string password, out string message)
        {
            string accoutInput = account.Trim();
            string passwordInput = password.Trim();
            if (!accoutDic.TryGetValue(accoutInput, out string stored))
            {
                message = "账号不存在";
                return false;
            }
            if (stored != passwordInput)
            {
                message = "密码错误";
                return false;
            }
            CurrentUserName = accoutInput;
            message = "登录成功";
            return true;
        }
    }

}
