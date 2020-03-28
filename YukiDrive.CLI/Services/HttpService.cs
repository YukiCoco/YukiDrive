using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YukiDrive.CLI.Helpers;

namespace YukiDrive.CLI.Services
{
    public class HttpService
    {
        HttpClient httpClient;
        SettingService setting;
        public HttpService(HttpClient httpClient,SettingService setting)
        {
            this.httpClient = httpClient;
            this.setting = setting;
        }

        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="FileName"></param>
        public void UploadFile(string url, string fileName, int maxBuffer = 6553600)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("找不到文件所在路径");
            //httpClient.DefaultRequestHeaders.Add("Content-Range","bytes ");
            var fileStream = File.OpenRead(fileName);
            if (!fileStream.CanRead)
            {
                throw new ArgumentException("缺少文件读取权限");
            }
            Console.WriteLine($"开始上传文件 {Path.GetFileName(fileName)}");
            int result = 0;
            long offset = 0;
            //判断文件大小
            if(maxBuffer > fileStream.Length)
                maxBuffer = (int)fileStream.Length;
            do
            {
                byte[] buffer = new byte[maxBuffer];
                result = fileStream.Read(buffer, 0, maxBuffer);
                long nextBytes = offset + result;
                if (result > 0)
                {
                    ByteArrayContent uploadContent = new ByteArrayContent(buffer.Take(result).ToArray());
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
                    //添加 Content Header
                    uploadContent.Headers.Add("Content-Range", $"bytes {offset}-{nextBytes - 1}/{fileStream.Length}");
                    uploadContent.Headers.Add("Content-Length", $"{result}");
                    requestMessage.Content = uploadContent;
                    //上传文件
                    var response = httpClient.SendAsync(requestMessage).Result;
                    //计算进度
                    double progress = Math.Round((double)(offset + result) / (double)fileStream.Length, 2);
                    Console.WriteLine($"文件 {Path.GetFileName(fileName)} ：当前上传进度为：{progress * 100}%");
                    offset += result;
                }
            }
            while (result > 0);
        }

        /// <summary>
        /// 获取上传路径
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="uploadPassword"></param>
        public async Task<string> GetUploadUrl(string uploadPath, string siteName = "onedrive")
        {
            string requestUrl = $"{setting.settings.ApiUrl}/api/cli/upload/{siteName}/:/{uploadPath}?uploadPassword={setting.settings.UploadPassword}";
            var response = httpClient.GetAsync(requestUrl).Result;
            string responseStr = await response.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(responseStr);
            if(response.IsSuccessStatusCode) {
                return o["requestUrl"].ToString();
            } else {
                throw new Exception(o["message"].ToString());
            }
        }

        private readonly object o = new object();
        /// <summary>
        /// Upload Folder by muti-thread
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="remoteFolderName"></param>
        /// <param name="siteName"></param>
        /// <param name="threadCount"></param>
        public void UploadFolder(string folderName, string remoteFolderName, string siteName, int threadCount = 1)
        {
            if (!Directory.Exists(folderName)) {
                System.Console.WriteLine("folder not found.");
                return;
            }
            string[] files = Directory.GetFiles(folderName);
            TaskScheduler taskScheduler = new LimitedConcurrencyLevelTaskScheduler(threadCount);
            TaskFactory taskFactory = new TaskFactory(taskScheduler);
            int fileCount = files.Count();
            foreach (var item in files)
            {
                taskFactory.StartNew(() =>
                {
                    string uploadPath = Path.Combine(remoteFolderName,Path.GetFileName(item));
                    string uploadUrl = this.GetUploadUrl(uploadPath, siteName).Result;
                    this.UploadFile(uploadUrl, item);
                    lock (o)
                    {
                        fileCount --;
                    }
                });
            }
            while (true)
            {
                if(fileCount == 0) {
                    System.Console.WriteLine("文件夹上传完毕");
                    return;
                }
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
        }
    }
}