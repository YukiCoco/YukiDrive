using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
namespace YukiDrive.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController
    {
        /// <summary>
        /// 重定向到 M$ 的 Oauth
        /// </summary>
        /// <returns></returns>
        [HttpGet("bind/url")]
        public async Task<RedirectResult> RedirectToBinding()
        {
            IConfidentialClientApplication app;
            app = ConfidentialClientApplicationBuilder
            .Create(Configuration.ClientId)
            .WithClientSecret(Configuration.ClientSecret)
            .WithRedirectUri(Configuration.RedirectUri)
            .Build();
            var redirectUrl = await app.GetAuthorizationRequestUrl(Configuration.Scopes).ExecuteAsync();
            var result = new RedirectResult(redirectUrl.AbsoluteUri);
            return result;
        }
    }
}