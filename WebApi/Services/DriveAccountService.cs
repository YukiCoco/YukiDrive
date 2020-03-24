using System.Threading;
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
        private AuthenticationResult authorizeResult;
        private AuthorizationCodeProvider authProvider;
        public SiteContext SiteContext { get; set; }
        /// <summary>
        /// Graph实例
        /// </summary>
        /// <value></value>
        public Microsoft.Graph.GraphServiceClient Graph { get; set; }

        private TokenService tokenService;
        public DriveAccountService(SiteContext siteContext,TokenService tokenService)
        {
            this.SiteContext = siteContext;
            this.tokenService = tokenService;
            this.app = tokenService.app;
            this.authorizeResult = tokenService.authorizeResult;
            this.authProvider = tokenService.authProvider;
            this.Graph = tokenService.Graph;
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
        /// 添加 SharePoint Site-ID 到数据库
        /// </summary>
        /// <param name="siteName"></param>
        /// <param name="dominName"></param>
        /// <returns></returns>
        public async Task AddSiteId(string siteName, string nickName)
        {
            Site site = new Site();
            //使用 Onedrive
            if(siteName == "onedrive"){
                site.Name = siteName;
                site.NickName = nickName;
            }
            else
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(20);
                    var apiCaller = new ProtectedApiCallHelper(httpClient);
                    await apiCaller.CallWebApiAndProcessResultASync($"{Configuration.GraphApi}/v1.0/sites/{Configuration.DominName}:/sites/{siteName}", authorizeResult.AccessToken, (result) =>
                    {
                        site.SiteId = result.Properties().Single((prop) => prop.Name == "id").Value.ToString();
                        site.Name = result.Properties().Single((prop) => prop.Name == "name").Value.ToString();
                        site.NickName = nickName;
                    });
                }
            }
            if (!SiteContext.Sites.Any(s => s.SiteId == site.SiteId))
            {
                //若是首次添加则设置为默认的驱动器
                using (SettingService setting = new SettingService(new SettingContext()))
                {
                    if (SiteContext.Sites.Count() == 0)
                    {
                        await setting.Set("DefaultDrive", site.Name);
                    }
                }
                await SiteContext.Sites.AddAsync(site);
                await SiteContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("站点已被创建");
            }
        }

        public List<Site> GetSites()
        {
            List<Site> result = SiteContext.Sites.ToList();
            return result;
        }

        /// <summary>
        /// 获取 Drive Info
        /// </summary>
        /// <returns></returns>
        public async Task<List<DriveInfo>> GetDriveInfo()
        {
            List<DriveInfo> drivesInfo = new List<DriveInfo>();
            foreach (var item in SiteContext.Sites.ToArray())
            {
                Microsoft.Graph.Drive drive;
                //Onedrive
                if (string.IsNullOrEmpty(item.SiteId))
                {
                    drive = await Graph.Me.Drive.Request().GetAsync();
                }
                else
                {
                    drive = await Graph.Sites[item.SiteId].Drive.Request().GetAsync();
                }
                drivesInfo.Add(new DriveInfo()
                {
                    Quota = drive.Quota,
                    NickName = item.NickName,
                    Name = item.Name,
                    HiddenFolders = item.HiddenFolders
                });
            }
            return drivesInfo;
        }

        public async Task Unbind(string nickName)
        {
            SiteContext.Sites.Remove(SiteContext.Sites.Single(site => site.NickName == nickName));
            await SiteContext.SaveChangesAsync();
        }
        public class DriveInfo
        {
            public Microsoft.Graph.Quota Quota { get; set; }
            public string NickName { get; set; }
            public string Name { get; set; }
            public string[] HiddenFolders { get; set; }
        }
    }
}