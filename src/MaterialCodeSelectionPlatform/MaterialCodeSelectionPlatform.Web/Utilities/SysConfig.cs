namespace MaterialCodeSelectionPlatform.Web.Utilities
{
    public class SysConfig
    {
        /// <summary>
        /// 域账户的域名
        /// </summary>
        public static string Domain { get; set; }

        /// <summary>
        /// 是否启用权限控制
        /// </summary>
        public static bool IsNeedPermission { get; set; } = true;

        /// <summary>
        /// 同步服务地址
        /// </summary>
        public static string SysServiceUrl { get; set; }
    }
}