using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using YukiDrive.Models;
using YukiDrive.Services;

namespace YukiDrive.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private IDriveAccountService driveAccount;
        private SettingService setting;
        public AdminController(IDriveAccountService driveAccount, SettingService setting)
        {
            this.driveAccount = driveAccount;
            this.setting = setting;
        }
        /// <summary>
        /// 重定向到 M$ 的 Oauth
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("bind/url")]
        public async Task<RedirectResult> RedirectToBinding()
        {
            string url = await driveAccount.GetAuthorizationRequestUrl();
            var result = new RedirectResult(url);
            return result;
        }
        /// <summary>
        /// 从 Oauth 重定向的url
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("bind/new")]
        public async Task<IActionResult> NewBinding(string code, string session_state)
        {
            try
            {
                var result = await driveAccount.Authorize(code);
                if (result.AccessToken != null)
                {
                    await setting.Set("AccountStatus", "已认证");
                    return Redirect("/#/admin");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = ex.Message
                });
            }
            return StatusCode(500, new Models.ErrorResponse()
            {
                Message = "未知错误"
            });
        }
        /// <summary>
        /// 添加 SharePoint Site
        /// </summary>
        /// <returns></returns>
        [HttpPost("sites")]
        public async Task<IActionResult> AddSite(AddSiteModel model)
        {
            try
            {
                await driveAccount.AddSiteId(model.siteName, model.nickName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = ex.Message
                });
            }
            return StatusCode(201);
        }

        /// <summary>
        /// 获取基本内容
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public async Task<IActionResult> GetInfo()
        {
            try
            {
                List<DriveAccountService.DriveInfo> driveInfo = new List<DriveAccountService.DriveInfo>();
                if (setting.Get("AccountStatus") == "已认证")
                {
                    driveInfo = await driveAccount.GetDriveInfo();
                }
                return Ok(new
                {
                    officeName = Configuration.AccountName,
                    officeType = Enum.GetName(typeof(Configuration.OfficeType), Configuration.Type),
                    driveInfo = driveInfo,
                    appName = setting.Get("AppName"),
                    webName = setting.Get("WebName"),
                    navImg = setting.Get("NavImg"),
                    defaultDrive = setting.Get("DefaultDrive"),
                    accountStatus = setting.Get("AccountStatus"),
                    readme = setting.Get("Readme")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// 设置readme
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("readme")]
        public async Task<IActionResult> UpdateReadme(ReadmeModel model)
        {
            try
            {
                await setting.Set("Readme", model.text);
            }
            catch (Exception e)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = e.Message
                });
            };
            return StatusCode(204);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        [HttpPost("settings")]
        public async Task<IActionResult> UpdateSetting(UpdateSettings toSaveSetting)
        {
            try
            {
                await setting.Set("AppName", toSaveSetting.appName);
                await setting.Set("WebName", toSaveSetting.webName);
                await setting.Set("NavImg", toSaveSetting.navImg);
                await setting.Set("DefaultDrive", toSaveSetting.defaultDrive);
            }
            catch (Exception e)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = e.Message
                });
            };
            return StatusCode(204);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [HttpDelete("sites")]
        public async Task<IActionResult> Unbind(string nickName)
        {
            try
            {
                await driveAccount.Unbind(nickName);
            }
            catch (Exception e)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = e.Message
                });
            };
            return StatusCode(204);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [HttpPost("sites/rename")]
        public async Task<IActionResult> SiteRename(SiteRenameModel model)
        {
            try
            {
                await driveAccount.SiteRename(model.oldName, model.nickName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Models.ErrorResponse()
                {
                    Message = ex.Message
                });
            }
            return StatusCode(204);
        }

        // /// <summary>
        // /// 上传文件
        // /// </summary>
        // /// <returns></returns>
        // public async Task<IActionResult> UploadFile(IFormFile file){

        // }

        #region 接收表单模型
        public class UpdateSettings
        {
            public string appName { get; set; }
            public string webName { get; set; }
            public string navImg { get; set; }
            public string defaultDrive { get; set; }
            public string readme { get; set; }
        }

        public class AddSiteModel
        {
            public string siteName { get; set; }
            public string nickName { get; set; }
        }

        public class SiteRenameModel
        {
            public string oldName { get; set; }
            public string nickName { get; set; }
        }

        public class ReadmeModel
        {
            public string text { get; set; }
        }
        #endregion
    }
}