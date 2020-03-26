using System.Net.Http;
using System;
using System.Net.Http.Headers;
using YukiDrive.CLI.Services;

namespace YukiDrive.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            if(args.Length == 0){
                System.Console.WriteLine("请输入命令");
                return;
            }
            SettingService settingService = new SettingService();
            HttpService httpService = new HttpService(new HttpClient(), settingService);
            switch (args[0])
            {
                //初始化
                //-init apiurl password
                case "-init":
                settingService.settings.ApiUrl = args[1];
                settingService.settings.UploadPassword = args[2];
                settingService.SaveSettings();
                System.Console.WriteLine("已更新设置");
                return;
                //上传
                //-upload sitename localpath uploadpath
                case "-upload":
                string uploadUrl = httpService.GetUploadUrl(args[3],args[1]).Result;
                httpService.UploadFile(uploadUrl,args[2]).Wait();
                break;
            }
        }
    }
}
