using System.Threading.Tasks;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using YukiDrive.Helpers;

namespace YukiDrive.Services
{
    public class DriveAccountService
    {
        private IConfidentialClientApplication app;
        public DriveAccountService(){
            app = ConfidentialClientApplicationBuilder
            .Create(Configuration.ClientId)
            .WithClientSecret(Configuration.ClientSecret)
            .WithRedirectUri(Configuration.RedirectUri)
            .Build();
            //缓存Token
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);
        }

        /// <summary>
        /// 返回 Oauth 验证url
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAuthorizationRequestUrl(){
            var redirectUrl = await app.GetAuthorizationRequestUrl(Configuration.Scopes).ExecuteAsync();
            return redirectUrl.AbsoluteUri;
        }
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> Authorize(string code){
            AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(app);
            var result = await authProvider.ClientApplication.AcquireTokenByAuthorizationCode(Configuration.Scopes, code).ExecuteAsync();
            return result;
        }
    }
}