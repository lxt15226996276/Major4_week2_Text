using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam04
{
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

        /// <summary>
        /// 注册：写入字典
        /// </summary>
        /// <returns></returns>
        public bool Register(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password)) return false;
            if (accountDic.ContainsKey(account))
            {
                Debug.Log("账号已存在，注册失败");
                return false;
            }
            accountDic[account] = password;
            Debug.Log($"[注册成功] {account},当前数量={accountDic.Count}");
            return true;
        }

        /// <summary>
        /// 字典对比
        /// </summary>
        /// <returns></returns>
        public bool TryLogin(string account, string password)
        {
            return accountDic.TryGetValue(account, out string storedPwd) && storedPwd == password;
        }

        public int Count => accountDic.Count;
    }

}

