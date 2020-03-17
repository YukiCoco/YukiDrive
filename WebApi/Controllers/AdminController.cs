using System;
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
        public AdminController(IDriveAccountService driveAccount,SettingService setting){
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
        public async Task<IActionResult> NewBinding(string code,string session_state)
        {
            try
            {
                var result = await driveAccount.Authorize(code);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("success");
        }

        /// <summary>
        /// 添加 SharePoint Site
        /// </summary>
        /// <returns></returns>
        [HttpPost("site")]
        public async Task<IActionResult> AddSite(string siteName){
            try{
                await driveAccount.AddSiteId(siteName);
            }
            catch(Exception ex){
                return BadRequest(ex.Message);
            }
            return Ok("success");
        }

        /// <summary>
        /// 获取基本内容
        /// </summary>
        /// <returns></returns>
        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(){
            var driveInfo = await driveAccount.GetDriveInfo();
            return Ok(new {
                officeName = Configuration.AccountName,
                officeType = Configuration.Type,
                driveInfo = driveInfo,
                appName = setting.Get("AppName"),
                webName = setting.Get("WebName"),
                navImg = setting.Get("NavImg")
            });
        }
    }
}