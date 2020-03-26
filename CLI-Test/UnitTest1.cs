using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YukiDrive.CLI.Services;

namespace YukiDrive.CLI.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFile()
        {
            string fileName = "/Users/yukino/Desktop/NewFile.txt";
            int maxBuffer = 6553600; //6400 kib 微软推荐使用
            if(!File.Exists(fileName))
                throw new ArgumentException("找不到文件所在路径");
            
            //httpClient.DefaultRequestHeaders.Add("Content-Range","bytes ");
            var fileStream = File.OpenRead(fileName);
            if(!fileStream.CanRead){
                throw new ArgumentException("缺少文件读取权限");
            }
            int result = 0;
            long offset = 0;
            //判断文件大小
            if(maxBuffer > fileStream.Length)
                maxBuffer = (int)fileStream.Length;
            //Debug.WriteLine(fileStream.Length);
            do {
                byte[] fileBytes = new byte[maxBuffer];
                result = fileStream.Read(fileBytes,0,maxBuffer);
                //ByteArrayContent uploadContent = new ByteArrayContent(fileBytes);
                //await httpClient.PutAsync(url,uploadContent);
                long nextBytes = offset + result;
                if(result > 0){
                    Debug.WriteLine("Content-Range",$"{offset}-{nextBytes - 1}/{fileStream.Length}");
                    double progress = Math.Round((double)(offset + result)/(double)fileStream.Length,2);
                    Debug.WriteLine($"当前上传进度为：{progress}");
                    offset += result;
                }
            }
            while(result > 0);
        }
        [TestMethod]
        public void TestMethod1(){
            byte[] bytes = new byte[3];
            Debug.WriteLine(bytes.Length);
        }

        [TestMethod]
        public void TestService(){
            HttpService service = new HttpService(new HttpClient(),new SettingService());
            string uploadUrl = service.GetUploadUrl("upload/October - Time To Love.mp3").Result;
            Debug.WriteLine(uploadUrl);
            service.UploadFile(uploadUrl,"/Users/yukino/Desktop/October - Time To Love.mp3").Wait();
        }
    }
}
