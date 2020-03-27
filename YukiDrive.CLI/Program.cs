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
            if(args.Length == 0){
                System.Console.WriteLine("未输入命令");
                return;
            }
            SettingService settingService = new SettingService();
            HttpService httpService = new HttpService(new HttpClient(), settingService);
            switch (args[0])
            {
                //初始化
                //-init apiurl password
                case "--init":
                settingService.settings.ApiUrl = args[1];
                settingService.settings.UploadPassword = args[2];
                settingService.SaveSettings();
                System.Console.WriteLine("已更新设置");
                return;
                //上传
                //-upload sitename localpath uploadpath
                case "--upload":
                System.Console.WriteLine("开始上传文件");
                string uploadUrl = httpService.GetUploadUrl(args[3],args[1]).Result;
                httpService.UploadFile(uploadUrl,args[2]);
                break;
                case "--upload-folder":
                //-upload sitename localpath uploadpath thread
                System.Console.WriteLine("开始上传文件夹");
                int threadCount = 1;
                if(args.Length == 5) {
                    threadCount = int.Parse(args[4]);
                }
                httpService.UploadFolder(args[2],args[3],args[1],threadCount);
                break;
                default:
                System.Console.WriteLine("无效的命令");
                break;
            }
        }
    }
}
