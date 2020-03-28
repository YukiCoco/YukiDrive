using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YukiDrive.Helpers;
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
        private readonly ILogger logger;
        public DefaultController(IDriveAccountService siteService, IDriveService driveService, SettingService setting,ILogger<DefaultController> logger)
        {
            this.siteService = siteService;
            this.driveService = driveService;
            this.setting = setting;
            this.logger = logger;
        }


        #region Actions
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
            bool isAdmin = false;
            string token = Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token))
            {
                if (Helpers.AuthenticationHelper.VerifyToken(token))
                {
                    isAdmin = true;
                }
            }
            if (string.IsNullOrEmpty(path))
            {
                try
                {
                    var result = await driveService.GetRootItems(siteName, isAdmin);
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
            else
            {
                var result = await driveService.GetDriveItemsByPath(path, siteName, isAdmin);
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
            bool isAollowAnonymous = string.IsNullOrEmpty(setting.Get("AllowAnonymouslyUpload")) ? false : Convert.ToBoolean(setting.Get("AllowAnonymouslyUpload"));
            return Ok(new
            {
                appName = setting.Get("AppName"),
                webName = setting.Get("WebName"),
                defaultDrive = setting.Get("DefaultDrive"),
                readme = setting.Get("Readme"),
                footer = setting.Get("Footer"),
                allowUpload = isAollowAnonymous
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

        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public IActionResult Test(){
            string token = Request.Headers["Authorization"];
            bool isAdmin = false;
            if (token != null)
            {
                if (Helpers.AuthenticationHelper.VerifyToken(token))
                {
                    isAdmin = true;
                }
            }
            return Ok(isAdmin);
        }

        /// <summary>
        /// 获取文件分片上传路径
        /// </summary>
        /// <returns></returns>
        [HttpGet("upload/{siteName}/{**fileName}")]
        public async Task<IActionResult> GetUploadUrl(string siteName, string fileName)
        {
            bool isAollowAnonymous = string.IsNullOrEmpty(setting.Get("AllowAnonymouslyUpload")) ? false : Convert.ToBoolean(setting.Get("AllowAnonymouslyUpload"));
            bool isAdmin = false;
            if (!isAollowAnonymous)
            {
                if (Request.Headers.ContainsKey("Authorization"))
                {
                    isAdmin = AuthenticationHelper.VerifyToken(Request.Headers["Authorization"]);
                    if (!isAdmin)
                    {
                        return Unauthorized(new ErrorResponse()
                        {
                            Message = "未经授权的访问"
                        });
                    }

                }
                else
                {
                    return Unauthorized(new ErrorResponse()
                    {
                        Message = "未经授权的访问"
                    });
                }
            }
            string path = Path.Combine($"upload/{Guid.NewGuid().ToString()}",fileName);
            try
            {
                var result = await driveService.GetUploadUrl(path, siteName);
                return Ok(new {
                    requestUrl = result,
                    fileUrl = $"{Configuration.BaseUri}/api/files/{siteName}/{path}"
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// 获取文件分片上传路径
        /// </summary>
        /// <returns></returns>
        [HttpGet("cli/upload/{siteName}/:/{**path}")]
        public async Task<IActionResult> GetUploadUrl(string siteName, string path, string uploadPassword)
        {
            if (uploadPassword != setting.Get("UploadPassword"))
            {
                return Unauthorized(new ErrorResponse()
                {
                    Message = "上传密码错误"
                });
            }
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest(new ErrorResponse()
                {
                    Message = "必须存在上传路径"
                });
            }
            try
            {
                var result = await driveService.GetUploadUrl(path, siteName);
                return Ok(new {
                    requestUrl = result
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        #endregion
    }
}