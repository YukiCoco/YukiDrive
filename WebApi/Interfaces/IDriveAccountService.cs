using System.Threading.Tasks;
using Microsoft.Identity.Client;

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
        public Task AddSiteId(string siteName);
    }
}