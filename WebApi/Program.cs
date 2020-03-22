using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YukiDrive.Services;
using YukiDrive.Models;
using YukiDrive.Contexts;
using Microsoft.AspNetCore;
using NLog.Web;

namespace YukiDrive
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //首次启动初始化
            Init();
            //忘记密码
            if (args.SingleOrDefault(str => str.Contains("newPassword:")) != null)
            {
                string pw = args.Single(str => str.Contains("newPassword:"));
                ChangePassword(pw);
            }
            //初始化 Logger
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder().Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
            System.Console.WriteLine("程序已启动");
        }
        /// <summary>
        /// 创建主机
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
            .UseUrls(Configuration.Urls) //使用自定义url
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .UseNLog()
            .UseStartup<Startup>();
        }

        public static void Init()
        {
            //初始化
            if (!File.Exists("YukiDrive.db"))
            {
                File.Copy("YukiDrive.template.db", "YukiDrive.db");
                System.Console.WriteLine("数据库创建成功");
            }
            using (SettingService settingService = new SettingService(new SettingContext()))
            {
                if (settingService.Get("IsInit") != "true")
                {
                    using (UserService userService = new UserService(new UserContext()))
                    {
                        User adminUser = new User()
                        {
                            Username = YukiDrive.Configuration.AdminName
                        };
                        userService.Create(adminUser, YukiDrive.Configuration.AdminPassword);
                    }
                    settingService.Set("IsInit", "true").Wait();
                    System.Console.WriteLine($"管理员初始名称：{Configuration.AdminName}");
                    System.Console.WriteLine($"管理员初始密码：{Configuration.AdminPassword}");
                    System.Console.WriteLine($"请登录 {Configuration.BaseUri}/#/login 进行身份认证");
                }
            }
        }

        public static void ChangePassword(string newPassword)
        {
            using (UserService userService = new UserService(new UserContext()))
            {
                userService.Update(userService.GetByUsername(Configuration.AdminName), YukiDrive.Configuration.AdminPassword);
            }
            System.Console.WriteLine("密码更新成功");
        }

        static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // use logger here to log the events exception object
            // before the application quits
        }
    }
}
