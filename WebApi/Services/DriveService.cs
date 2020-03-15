using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using YukiDrive.Models;
using System.Threading.Tasks;
using YukiDrive.Helpers;

namespace YukiDrive.Services
{
    public class DriveService
    {
        private IDriveRequestBuilder drive;
        public IConfidentialClientApplication app;
        public DriveService(IDriveRequestBuilder drive)
        {
            this.drive = drive;
            app = ConfidentialClientApplicationBuilder
            .Create(Configuration.ClientId)
            .WithClientSecret(Configuration.ClientSecret)
            .WithRedirectUri(Configuration.RedirectUri)
            .Build();
            //缓存Token
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);
        }
        /// <summary>
        /// 获取根目录的所有项目
        /// </summary>
        /// <returns></returns>
        public async Task<List<DriveFile>> GetRootItems()
        {
            var result = await drive.Root.Children.Request().GetAsync();
            List<DriveFile> files = SaveItems(result);
            return files;
        }
        /// <summary>
        /// 根据id获取文件夹下所有项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<DriveFile>> GetDriveItemsById(string id)
        {
            var result = await drive.Items[id].Children.Request().GetAsync();
            List<DriveFile> files = SaveItems(result);
            return files;
        }
        /// <summary>
        /// 根据路径获取文件夹下所有项目
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<List<DriveFile>> GetDriveItemsByPath(string path)
        {
            var result = await drive.Root.ItemWithPath(path).Children.Request().GetAsync();
            List<DriveFile> files = SaveItems(result);
            return files;
        }
        /// <summary>
        /// 根据路径获取项目
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<DriveFile> GetDriveItemByPath(string path)
        {
            var result = await drive.Root.ItemWithPath(path).Request().GetAsync();
            DriveFile file = SaveItem(result);
            return file;
        }
        /// <summary>
        /// 根据id获取项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<DriveFile> GetDriveItemById(string id)
        {
            var result = await drive.Items[id].Request().GetAsync();
            DriveFile file = SaveItem(result);
            return file;
        }
        #region PrivateMethod
        private DriveFile SaveItem(DriveItem result)
        {
            DriveFile file = new DriveFile()
            {
                CreatedTime = result.CreatedDateTime,
                Name = result.Name,
                Size = result.Size,
                Id = result.Id
            };
            object downloadUrl;
            if (result.AdditionalData != null)
            {
                //可能是文件夹
                if (result.AdditionalData.TryGetValue("@microsoft.graph.downloadUrl", out downloadUrl))
                    file.DownloadUrl = (string)downloadUrl;
            }

            return file;
        }

        private List<DriveFile> SaveItems(IDriveItemChildrenCollectionPage result)
        {
            List<DriveFile> files = new List<DriveFile>();
            foreach (var item in result)
            {
                DriveFile file = new DriveFile()
                {
                    CreatedTime = item.CreatedDateTime,
                    Name = item.Name,
                    Size = item.Size,
                    Id = item.Id
                };
                object downloadUrl;
                if (item.AdditionalData != null)
                {
                    //可能是文件夹
                    if (item.AdditionalData.TryGetValue("@microsoft.graph.downloadUrl", out downloadUrl))
                        file.DownloadUrl = (string)downloadUrl;
                }
                files.Add(file);
            }

            return files;
        }

        #endregion

    }
}