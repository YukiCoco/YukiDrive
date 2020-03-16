using System.Collections.Generic;
using YukiDrive.Models;

namespace YukiDrive.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
    }
}