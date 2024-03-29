﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MaterialCodeSelectionPlatform.Web.Utilities;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class LoginController : Controller
    {
        private IUserService userService;
        private string domain;
        public LoginController(IUserService userService,IConfiguration configuration)
        {
            this.userService = userService;
            domain = configuration["Domain"].ToLower();
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
        public async Task<IActionResult> LoginUserName(string userName, string password)
        {
            var datePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MaterWebDate.dll");
            if (System.IO.File.Exists(datePath))
            {
                var lastDate = DateTime.Parse(System.IO.File.ReadAllText(datePath));
                if (DateTime.Now < lastDate)
                {
                    return Content("登录失败");
                }

                if (DateTime.Now > DateTime.Parse("2021-03-01"))
                {
                    return Content("登录失败");
                }
            }
            System.IO.File.WriteAllText(datePath,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
          



            ///域登录一般的用户名形式为
            ///  domain\userName  或者 userName@domain
            if (userName == null)
            {
                return Content("用户名或密码错误");
            }

            userName = userName.ToLower();
            if (userName.StartsWith(domain))
            {
                
                if (LDAPUtil.Validate(userName.Replace(domain+"\\",""), password))
                {
                    var list = await userService.GetByParentId("DomainUserName", userName);
                    if (list.Count > 0)
                    {
                        var user = list.FirstOrDefault();
                        var claimsIdentity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Id),
                            new Claim("LoginName", user.LoginName),
                            new Claim("UserName", user.Name),
                        }, "Basic");

                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        HttpContext.Session.SetString("LoginName", user.LoginName ?? Guid.Empty.ToString());
                        HttpContext.Session.SetString("UserName", user.Name ?? Guid.Empty.ToString());
                        HttpContext.Session.SetString("UserId", user.Id ?? Guid.Empty.ToString());
                        if (user.Role.HasValue)
                            HttpContext.Session.SetString("Role", user.Role.Value.ToString() ?? "");
                        return Content("success");
                    }
                }
                return Content("用户名或密码错误");
            }
            else
            {

                var user = await userService.GetByUserNamePwd(userName, CommonHelper.ToMD5(password));
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
                    if(user.Role.HasValue)
                        HttpContext.Session.SetString("Role", user.Role.Value.ToString()??"");

                    return Content("success");
                }
            }


            //var user = await userService.GetByUserNamePwd(userName, CommonHelper.ToMD5(password));
            //if (user == null)
            //{
            //    //判断是否为域账户登录
            //    if (LDAPUtil.Validate(userName, password))
            //    {
            //        var list = await userService.GetByParentId("DomainUserName", userName);
            //        if (list.Count > 0)
            //        {
            //            user = list.FirstOrDefault();
            //            var claimsIdentity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Id),
            //                new Claim("LoginName", user.LoginName),
            //                new Claim("UserName", user.Name),
            //            }, "Basic");

            //            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            //            HttpContext.Session.SetString("LoginName", user.LoginName ?? Guid.Empty.ToString());
            //            HttpContext.Session.SetString("UserName", user.Name ?? Guid.Empty.ToString());
            //            HttpContext.Session.SetString("UserId", user.Id ?? Guid.Empty.ToString());
            //            return Content("success");
            //        }
            //    }
            //    return Content("用户名或密码错误");
            //}
            //else
            //{
            //    var claimsIdentity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, user.Id),
            //        new Claim("LoginName", user.LoginName),
            //        new Claim("UserName", user.Name),
            //    }, "Basic");

            //    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            //    HttpContext.Session.SetString("LoginName", user.LoginName ?? Guid.Empty.ToString());
            //    HttpContext.Session.SetString("UserName", user.Name ?? Guid.Empty.ToString());
            //    HttpContext.Session.SetString("UserId", user.Id ?? Guid.Empty.ToString());
            //    return Content("success");
            //}
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

