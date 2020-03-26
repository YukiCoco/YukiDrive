using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public async Task UploadFile(string url, string fileName, int maxBuffer = 6553600)
        {
            if (!File.Exists(fileName))
                throw new ArgumentException("找不到文件所在路径");
            //httpClient.DefaultRequestHeaders.Add("Content-Range","bytes ");
            var fileStream = File.OpenRead(fileName);
            if (!fileStream.CanRead)
            {
                throw new ArgumentException("缺少文件读取权限");
            }
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
                    var response = await httpClient.SendAsync(requestMessage);
                    //计算进度
                    double progress = Math.Round((double)(offset + result) / (double)fileStream.Length, 2);
                    Console.WriteLine($"当前上传进度为：{progress}");
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
            var response = await httpClient.GetAsync(requestUrl);
            string responseStr = await response.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(responseStr);
            if(response.IsSuccessStatusCode) {
                return o["requestUrl"].ToString();
            } else {
                throw new Exception(o["message"].ToString());
            }
        }
    }
}