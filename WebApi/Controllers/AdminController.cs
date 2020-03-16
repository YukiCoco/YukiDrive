using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using YukiDrive.Models;
using YukiDrive.Services;

namespace YukiDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private IDriveAccountService driveAccount;
        public AdminController(IDriveAccountService driveAccount){
            this.driveAccount = driveAccount;
        }
        /// <summary>
        /// 重定向到 M$ 的 Oauth
        /// </summary>
        /// <returns></returns>
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
    }
}