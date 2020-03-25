using System.Net.Http;
using System.IO;

namespace YukiDrive.CLI.Services
{
    public class HttpService
    {
        HttpClient httpClient;
        public HttpService(HttpClient httpClient)
        {
            httpClient.GetAsync();
            File file = new File();
        }
    }
}