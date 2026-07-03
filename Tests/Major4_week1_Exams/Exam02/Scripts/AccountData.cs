using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Exam.Exam02
{
    /// <summary>
    /// 全局账号数据：Dictionary 存账号和密码，跨场景不销毁
    /// </summary>
    public class AccountData : MonoBehaviour
    {
        public static AccountData Instance { get; private set; }

        private Dictionary<string, string> accountDic = new Dictionary<string, string>();
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool Register(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password)) return false;

            accountDic[account] = password;
            Debug.Log($"[注册成功] 账号={account} ,当前总数={accountDic.Count}");
            return true;
        }

        public bool TryLogin(string account, string password)
        {
            return accountDic.TryGetValue(account, out string storedPwd) && storedPwd == password;
        }

        public int Count=>accountDic.Count;


        
    }
}

