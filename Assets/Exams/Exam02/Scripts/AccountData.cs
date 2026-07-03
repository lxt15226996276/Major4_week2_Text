using System.Collections.Generic;
using UnityEngine;
namespace Exam.Exam02
{
   public class AccountData : MonoBehaviour
   {
      //单利
      public static AccountData Instance { get; private set; }
      //字典
      private Dictionary<string, string> accountDic = new Dictionary<string, string>();

      void Awake()
      {
         if (Instance != null && Instance != this)
         {
            Destroy(gameObject);
            return;
         }
         Instance = this;

         DontDestroyOnLoad(gameObject);//目前没必要
      }
      /// <summary>
      /// 添加注册数据
      /// </summary>
      public bool Register(string account, string password, out string message)
      {
         //判空
         account = account?.Trim();
         password = password?.Trim();
         message = "";
         if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
         {
            message = "账号或密码不能为空";
            //Debug.Log("账号或密码为空，注册失败");
            return false;
         }
         if (accountDic.ContainsKey(account))
         {
            message = "账号已存在";
            return false;
         }
         //写入数据
         //accountDic[account] = password;
         accountDic.Add(account, password);
         message = $"[注册成功] account={account} 密码={password} ,当前注册总数：{accountDic.Count}";
         return true;
      }

      /// <summary>
      /// 尝试登录
      /// </summary>
      public bool TryLogin(string account, string password)
      {
         account = account?.Trim();
         password = password?.Trim();
         return accountDic.TryGetValue(account, out string stored) && stored == password;
      }



   }

}
