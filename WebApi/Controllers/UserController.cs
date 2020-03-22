using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using YukiDrive.Helpers;
using YukiDrive.Models;
using YukiDrive.Services;

namespace YukiDrive.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        #region Authentication Model
        public class AuthenticateModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }
        #endregion
        private IUserService userService { get; set; }
        public UserController(IUserService service)
        {
            this.userService = service;
        }
        /// <summary>
        /// 验证 返回Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateModel model)
        {
            var user = userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new ErrorResponse()
                {
                    Message = "错误的用户名或密码"
                });
            string tokenString = AuthenticationHelper.CreateToken(user);
            // return basic user info and authentication token
            return Ok(new
            {
                error = false,
                Id = user.Id,
                Username = user.Username,
                Token = tokenString
            });
        }
    }
}