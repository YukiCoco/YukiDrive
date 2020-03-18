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
        SettingService setting;


        public DefaultController(IDriveAccountService siteService, IDriveService driveService, SettingService setting)
        {
            this.siteService = siteService;
            this.driveService = driveService;
            this.setting = setting;
        }
        /// <summary>
        /// 返回所有sites
        /// </summary>
        /// <returns></returns>
        [HttpGet("site")]
        public IActionResult GetSites()
        {
            return Ok(siteService.GetSites());
        }
        /// <summary>
        /// 根据路径获取文件夹内容
        /// </summary>
        /// <returns></returns>
        [HttpGet("show/{siteName}/{**path}")]
        public async Task<IActionResult> GetDrectory(string siteName, string path)
        {
            if (string.IsNullOrEmpty(siteName))
            {
                return Ok(new Response()
                {
                    Error = true,
                    Message = "找不到请求的 Site Name"
                });
            }
            if (string.IsNullOrEmpty(path))
            {
                var result = await driveService.GetRootItems(siteName);
                return Ok(result);
            }
            else
            {
                var result = await driveService.GetDriveItemsByPath(path, siteName);
                if (result == null)
                {
                    return Ok(new Response()
                    {
                        Error = true,
                        Message = $"路径{path}不存在"
                    });
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
                return Ok(new Response()
                {
                    Error = true,
                    Message = $"路径{path}不存在"
                });
            }
        }

        /// <summary>
        /// 获取基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                appName = setting.Get("AppName"),
                webName = setting.Get("WebName"),
                navImg = setting.Get("NavImg")
            });
        }
    }
}