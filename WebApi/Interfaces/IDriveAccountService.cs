using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using YukiDrive.Models;
using static YukiDrive.Services.DriveAccountService;

namespace YukiDrive.Services
{
    public interface IDriveAccountService
    {
        /// <summary>
        /// 返回 Oauth 验证url
        /// </summary>
        /// <returns></returns>
        public Task<string> GetAuthorizationRequestUrl();
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<AuthenticationResult> Authorize(string code);
        /// <summary>
        /// 添加 SharePoint Site-ID
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="dominName"></param>
        /// <returns></returns>
        public Task<Response> AddSiteId(string siteName,string nickName);

        /// <summary>
        /// Graph实例
        /// </summary>
        /// <value></value>
        public Microsoft.Graph.GraphServiceClient Graph {get; set;}
        /// <summary>
        /// 返回所有 sharepoint site
        /// </summary>
        /// <returns></returns>
        public List<Site> GetSites();

        /// <summary>
        /// 获取驱动器信息
        /// </summary>
        /// <returns></returns>
        public Task<List<DriveInfo>> GetDriveInfo();

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public Task Unbind(string nickName);
    }
}