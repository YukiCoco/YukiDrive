using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System;

namespace YukiDrive.CLI.Services
{
    public class HttpService
    {
        HttpClient httpClient;
        public HttpService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
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
                byte[] fileBytes = new byte[maxBuffer];
                result = fileStream.Read(fileBytes, 0, maxBuffer);
                long nextBytes = offset + result;
                if (result > 0)
                {
                    
                    ByteArrayContent uploadContent = new ByteArrayContent(fileBytes);
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
        public void GetUploadUrl(string apiUrl,string uploadPassword){
            //httpClient.GetAsync()
        }
    }
}