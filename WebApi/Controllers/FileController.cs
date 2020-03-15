using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YukiDrive.Services;

namespace YukiDrive.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        IDriveService service;
        public FileController(IDriveService service){
            this.service = service;
        }

        [HttpGet("test")]
        public string Test(){
            System.Console.WriteLine(YukiDrive.Configuration.ConnectionString);
            return "233333";
        }

        // catch-all 参数匹配路径
        /// <summary>
        /// 返回下载路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("down/{siteName}/{**path}")]
        public async Task<IActionResult> Download(string siteName,string path){
            var result = await service.GetDriveItemByPath(path,siteName);
            return Ok(result);
        }
    }
}