using UnityEngine;
namespace Exam.Exam08
{
    public static class GameSession
    {
        //登录三字段
        public static string Account { get; set; }
        public static string Password { get; set; }
        public static string PlayerName { get; set; }
        //Loading 通用：进Loading前必须赋值
        public static string NextScene { get; set; }
        //选服
        public static string SelectedServer { get; set; }

    }

}
