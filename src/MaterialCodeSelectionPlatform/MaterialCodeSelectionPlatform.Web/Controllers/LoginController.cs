using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Top.WizEasc.ManagerWeb.Controllers
{
    public class LoginController : Controller
    {
        private IUserService userService;
        public LoginController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IActionResult> LoginUserName( string userName, string password)
        {
            var user = await userService.GetByUserNamePwd(userName, password);
            if (user == null)
            {
                return Content("用户名或密码错误");
            }
            else
            {
                var claimsIdentity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Id),
                    new Claim("LoginName", user.LoginName),
                    new Claim("UserName", user.Name),
                }, "Basic");

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                HttpContext.Session.SetString("LoginName", user.LoginName ?? Guid.Empty.ToString());
                HttpContext.Session.SetString("UserName", user.Name ?? Guid.Empty.ToString());
                HttpContext.Session.SetString("UserId", user.Id ?? Guid.Empty.ToString());
                return Content("success");
            }
        }


        /// <summary>
        /// 退出功能
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            //关闭时清除cookie
            HttpContext.Response.Cookies.Delete("vid");

            return Content("success");
        }
      


    }
}

