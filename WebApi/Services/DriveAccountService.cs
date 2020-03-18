using System.Diagnostics;
using System.IO;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using YukiDrive.Helpers;
using YukiDrive.Contexts;
using YukiDrive.Models;
using System.Net;
using System.Collections.Generic;

namespace YukiDrive.Services
{
    public class DriveAccountService : IDriveAccountService
    {
        private IConfidentialClientApplication app;
        public AuthenticationResult authorizeResult;
        private AuthorizationCodeProvider authProvider;
        private SiteContext siteContext;
        /// <summary>
        /// Graph实例
        /// </summary>
        /// <value></value>
        public Microsoft.Graph.GraphServiceClient Graph { get; set; }
        public DriveAccountService(SiteContext siteContext)
        {
            if (Configuration.Type == Configuration.OfficeType.China)
            {
                app = ConfidentialClientApplicationBuilder
            .Create(Configuration.ClientId)
            .WithClientSecret(Configuration.ClientSecret)
            .WithRedirectUri(Configuration.RedirectUri)
            .WithAuthority(AzureCloudInstance.AzureChina, "common")
            .Build();
            }
            else
            {
                app = ConfidentialClientApplicationBuilder
            .Create(Configuration.ClientId)
            .WithClientSecret(Configuration.ClientSecret)
            .WithRedirectUri(Configuration.RedirectUri)
            .WithAuthority(AzureCloudInstance.AzurePublic, "common")
            .Build();
            }
            //缓存Token
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);
            //这里要传入一个 Scope 否则默认使用 https://graph.microsoft.com/.default
            //而导致无法使用世纪互联版本
            authProvider = new AuthorizationCodeProvider(app, Configuration.Scopes);
            //获取Token
            if (File.Exists(TokenCacheHelper.CacheFilePath))
            {
                authorizeResult = authProvider.ClientApplication.AcquireTokenSilent(Configuration.Scopes, Configuration.AccountName).ExecuteAsync().Result;
                //Debug.WriteLine(authorizeResult.AccessToken);
            }
            //启用代理
            if (!string.IsNullOrEmpty(Configuration.Proxy))
            {
                // Configure your proxy
                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(Configuration.Proxy),
                    UseDefaultCredentials = true
                };
                var httpProvider = new Microsoft.Graph.HttpProvider(httpClientHandler, false)
                {
                    OverallTimeout = TimeSpan.FromSeconds(10)
                };
                Graph = new Microsoft.Graph.GraphServiceClient($"{Configuration.GraphApi}/v1.0", authProvider, httpProvider);
            }
            else
            {
                Graph = new Microsoft.Graph.GraphServiceClient($"{Configuration.GraphApi}/v1.0", authProvider);
            }
            this.siteContext = siteContext;
        }
        /// <summary>
        /// 返回 Oauth 验证url
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAuthorizationRequestUrl()
        {
            var redirectUrl = await app.GetAuthorizationRequestUrl(Configuration.Scopes).ExecuteAsync();
            return redirectUrl.AbsoluteUri;
        }
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> Authorize(string code)
        {
            AuthorizationCodeProvider authProvider = new AuthorizationCodeProvider(app);
            var result = await authProvider.ClientApplication.AcquireTokenByAuthorizationCode(Configuration.Scopes, code).ExecuteAsync();
            return result;
        }
        /// <summary>
        /// 添加 SharePoint Site-ID 到数据库
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="dominName"></param>
        /// <returns></returns>
        public async Task AddSiteId(string siteName)
        {
            Site site = new Site();
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                var apiCaller = new ProtectedApiCallHelper(httpClient);
                await apiCaller.CallWebApiAndProcessResultASync($"{Configuration.GraphApi}/v1.0/sites/{Configuration.DominName}:/sites/{siteName}", authorizeResult.AccessToken, (result) =>
                {
                    site.SiteId = result.Properties().Single((prop) => prop.Name == "id").Value.ToString();
                    site.Name = result.Properties().Single((prop) => prop.Name == "name").Value.ToString();
                });
            }
            await siteContext.Sites.AddAsync(site);
            await siteContext.SaveChangesAsync();
        }

        public List<Site> GetSites()
        {
            List<Site> result = siteContext.Sites.ToList();
            return result;
        }

        /// <summary>
        /// 获取 Drive Info
        /// </summary>
        /// <returns></returns>
        public async Task<List<DriveInfo>> GetDriveInfo(){
            List<DriveInfo> drivesInfo = new List<DriveInfo>();
            foreach (var item in siteContext.Sites.ToArray())
            {
                var drive = await Graph.Sites[item.SiteId].Drive.Request().GetAsync();
                drivesInfo.Add(new DriveInfo(){
                    Quota = drive.Quota,
                    NickName = item.NickName
                });
            }
            return drivesInfo;
        }

        public class DriveInfo{
            public Microsoft.Graph.Quota Quota { get; set; }
            public string NickName { get; set; }
        }
    }
}