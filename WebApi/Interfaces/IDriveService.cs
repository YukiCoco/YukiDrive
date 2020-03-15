using System.Collections.Generic;
using System.Threading.Tasks;
using YukiDrive.Models;

namespace YukiDrive.Services
{
    public interface IDriveService
    {
          public Task<List<DriveFile>> GetRootItems(string siteId);
          public Task<List<DriveFile>> GetDriveItemsById(string id,string siteId);
          public Task<List<DriveFile>> GetDriveItemsByPath(string path,string siteId);
          public Task<DriveFile> GetDriveItemByPath(string path,string siteId);
          public Task<DriveFile> GetDriveItemById(string id,string siteId);
    }
}