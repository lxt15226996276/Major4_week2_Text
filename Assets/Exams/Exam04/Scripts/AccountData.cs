using System.Collections.Generic;
using UnityEngine;

public class AccountData : MonoBehaviour
{
    /// <summary>
    /// 单利
    /// </summary>
    public static AccountData Instance;
    /// <summary>
    /// 字典
    /// </summary>
    private readonly Dictionary<string, string> _accoutDic = new Dictionary<string, string>();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="accout"></param>
    /// <param name="password"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool Register(string accout, string password, out string message)
    {
        accout = accout?.Trim();
        password = password?.Trim();
        if (string.IsNullOrEmpty(accout) || string.IsNullOrEmpty(password))
        {
            message = "账号或密码不能为空";
            return false;
        }
        if (_accoutDic.ContainsKey(accout))
        {
            message = "账号已存在";
            return false;
        }
        _accoutDic.Add(accout, password);
        message = "注册成功";
        return true;
    }
    /// <summary>
    /// 尝试登录
    /// </summary>
    /// <param name="accout"></param>
    /// <param name="password"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool TryLogin(string accout, string password, out string message)
    {
        accout = accout?.Trim();
        password = password?.Trim();
        if (string.IsNullOrEmpty(accout) || string.IsNullOrEmpty(password))
        {
            message = "账号或密码不能为空";
            return false;
        }
        if (!_accoutDic.TryGetValue(accout, out string stored))
        {
            message = "账号不存在";
            return false;
        }
        if (password != stored)
        {
            message = "密码错误";
            return false;
        }
        message = "登录成功";
        return true;
    }
}
