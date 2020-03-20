using System;
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
        [HttpGet("sites")]
        public IActionResult GetSites()
        {
            return Ok(siteService.GetSites());
        }
        /// <summary>
        /// 根据路径获取文件夹内容
        /// </summary>
        /// <returns></returns>
        [HttpGet("sites/{siteName}/{**path}")]
        public async Task<IActionResult> GetDrectory(string siteName, string path)
        {
            if (string.IsNullOrEmpty(siteName))
            {
                return NotFound(new ErrorResponse()
                {
                    Message = "找不到请求的 Site Name"
                });
            }
            if (string.IsNullOrEmpty(path))
            {
                try
                {
                    var result = await driveService.GetRootItems(siteName);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            else
            {
                var result = await driveService.GetDriveItemsByPath(path, siteName);
                if (result == null)
                {
                    return NotFound(new ErrorResponse()
                    {
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
        [HttpGet("files/{siteName}/{**path}")]
        public async Task<IActionResult> Download(string siteName, string path)
        {
            DriveFile result;
            try
            {
                result = await driveService.GetDriveItemByPath(path, siteName);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            if (result != null)
            {
                return new RedirectResult(result.DownloadUrl);
            }
            else
            {
                return NotFound(new ErrorResponse()
                {
                    Message = $"所求的{path}不存在"
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
                defaultDrive = setting.Get("DefaultDrive"),
                readme = setting.Get("Readme"),
                footer = setting.Get("Footer")
            });
        }
        /// <summary>
        /// 获得readme
        /// </summary>
        /// <returns></returns>
        [HttpGet("readme")]
        public IActionResult GetReadme()
        {
            return Ok(new
            {
                readme = setting.Get("Readme")
            });
        }
    }
}