using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace YukiDrive
{
    public class Configuration
    {
        private static IConfigurationRoot configurationRoot;
        static Configuration()
        {
            //throw new Exception(Directory.GetCurrentDirectory());
            //File.WriteAllText("debug.log",Directory.GetCurrentDirectory());
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, reloadOnChange: true);
            configurationRoot = config.Build();
        }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnectionString => configurationRoot["ConnectionString"];

        /// <summary>
        /// Graph连接 ClientID
        /// </summary>
        public static string ClientId => configurationRoot["ClientId"];

        /// <summary>
        /// Graph连接 ClientSecret
        /// </summary>
        public static string ClientSecret => configurationRoot["ClientSecret"];

        /// <summary>
        /// Binding 回调 Url
        /// </summary>
        public static string RedirectUri => configurationRoot["RedirectUri"];

        /// <summary>
        /// 返回 Scopes
        /// </summary>
        /// <value></value>
        public static string[] Scopes => new string[] { "Files.ReadWrite.All" };

        /// <summary>
        /// 代理路径
        /// </summary>
        public static string Proxy => configurationRoot["Proxy"];

        /// <summary>
        /// 账户名称
        /// </summary>
        public static string AccountName => configurationRoot["AccountName"];

        /// <summary>
        /// 域名
        /// </summary>
        public static string DominName => configurationRoot["DominName"];

        /// <summary>
        /// Office 类型
        /// </summary>
        /// <param name="="></param>
        /// <returns></returns>
        public static OfficeType Type => (configurationRoot["Type"] == "China") ? OfficeType.China : OfficeType.Global;

        /// <summary>
        /// Graph Api
        /// </summary>
        /// <param name="="></param>
        /// <returns></returns>
        public static string GraphApi => (configurationRoot["Type"] == "China") ? "https://microsoftgraph.chinacloudapi.cn" : "https://graph.microsoft.com";
        public enum OfficeType
        {
            Global,
            China
        }
    }
}