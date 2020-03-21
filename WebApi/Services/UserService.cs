using System;
using System.Collections.Generic;
using System.Linq;
using YukiDrive.Contexts;
using YukiDrive.Models;
using YukiDrive.Helpers;

namespace YukiDrive.Services
{
    public class UserService : IUserService, IDisposable
    {
        private UserContext userContext { get; set; }
        public UserService(UserContext context)
        {
            userContext = context;
        }

        public User Authenticate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return null;
            User user = userContext.Users.SingleOrDefault(user => user.Username == userName);
            if (user == null)
                return null;
            bool verify = AuthenticationHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);
            if (verify)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public User GetById(int id)
        {
            return userContext.Users.Find(id);
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (userContext.Users.Any(x => x.Username == user.Username))
                throw new Exception("Username \"" + user.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            AuthenticationHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            userContext.Users.Add(user);
            userContext.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = userContext.Users.Find(userParam.Id);

            if (user == null)
                throw new Exception("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (userContext.Users.Any(x => x.Username == userParam.Username))
                    throw new Exception("Username " + userParam.Username + " is already taken");

                user.Username = userParam.Username;
            }

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                AuthenticationHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            userContext.Users.Update(user);
            userContext.SaveChanges();
        }

        public void Dispose(){
            this.userContext.Dispose();
        }
    }
}