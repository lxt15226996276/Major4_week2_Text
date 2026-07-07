namespace Exam.Exam10
{
    /// <summary>
    /// Exam10 跨场景数据：登录三字段 + Loading 目标场景名。
    /// [继承 Exam08] Loading 读 NextScene 决定加载哪张场景。
    /// </summary>
    public static class GameSession
    {
        public static string Account { get; set; }
        public static string Password { get; set; }
        public static string PlayerName { get; set; }

        /// <summary>Loading 结束后要进的场景名（Login 后 = Server，选服后 = Main）</summary>
        public static string NextScene { get; set; }
        public static string SelectedServer { get; set; }
    }
}