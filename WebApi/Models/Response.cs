using System;
namespace YukiDrive.Models
{
    /// <summary>
    /// 请求结果
    /// </summary>
    public class Response
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public object Result { get; set; }
    }
}