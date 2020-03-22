using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace YukiDrive
{
    public class Configuration
    {
        private static IConfigurationRoot configurationRoot;
        private static IConfigurationBuilder builder;
        static Configuration()
        {
            //throw new Exception(Directory.GetCurrentDirectory());
            //File.WriteAllText("debug.log",Directory.GetCurrentDirectory());
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, reloadOnChange: true);
            configurationRoot = builder.Build();
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
        public static string BaseUri => configurationRoot["BaseUri"];

        /// <summary>
        /// 返回 Scopes
        /// </summary>
        /// <value></value>
        public static string[] Scopes => new string[] { "Sites.ReadWrite.All" ,"Files.ReadWrite.All" };

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

        /// <summary>
        /// 主机使用的URL
        /// </summary>
        public static string Urls => configurationRoot["ListeningUrls"];
        public enum OfficeType
        {
            Global,
            China
        }

        /// <summary>
        /// 验证密钥
        /// </summary>
        public static string Secret => configurationRoot["Secret"];

        /// <summary>
        /// 管理员名称
        /// </summary>
        public static string AdminName => configurationRoot["AdminName"];

        /// <summary>
        /// 管理员密码
        /// </summary>
        public static string AdminPassword => configurationRoot["AdminPassword"];

        /// <summary>
        /// Https 证书
        /// </summary>
        /// <typeparam name="Certificate"></typeparam>
        /// <returns></returns>
        public static Certificate HttpsCertificate => configurationRoot.GetSection("Certificate").Get<Certificate>();

        public class Certificate {
            public bool Enable { get; set; }
            public string FilePath { get; set; }
            public string Password { get; set; }
        }
    }
}