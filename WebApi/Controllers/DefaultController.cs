using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YukiDrive.Models;
using YukiDrive.Services;

namespace YukiDrive.Controllers
{
    [ApiController]
    [Route("api/")]
    public class DefaultController : ControllerBase
    {
        IDriveAccountService siteService;
        IDriveService driveService;
        public DefaultController(IDriveAccountService siteService,IDriveService driveService){
            this.siteService = siteService;
            this.driveService = driveService;
        }
        /// <summary>
        /// 返回所有sites
        /// </summary>
        /// <returns></returns>
        [HttpGet("site")]
        public IActionResult GetSites(){
            return Ok(siteService.GetSites());
        }
        /// <summary>
        /// 根据路径获取文件夹内容
        /// </summary>
        /// <returns></returns>
        [HttpGet("show/{siteName}/{path?}")]
        public async Task<IActionResult> GetDrectory(string siteName,string path){
            if(string.IsNullOrEmpty(path)){
                var result = await driveService.GetRootItems(siteName);
                return Ok(result);
            } else{
                var result = await driveService.GetDriveItemsByPath(path,siteName);
                if(result == null){
                    return NotFound($"路径{path}不存在");
                }
                return Ok(result);
            }
        }

        // catch-all 参数匹配路径
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("down/{siteName}/{**path}")]
        public async Task<IActionResult> Download(string siteName, string path)
        {
            var result = await driveService.GetDriveItemByPath(path, siteName);
            if (result != null)
            {
                return new RedirectResult(result.DownloadUrl);
            }
            else
            {
                return NotFound($"路径{path}不存在");
            }
        }
    }
}