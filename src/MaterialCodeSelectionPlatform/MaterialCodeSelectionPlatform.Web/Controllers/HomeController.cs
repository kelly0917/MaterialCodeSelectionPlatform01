using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Models;
using MaterialCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Web.Utilities;
using Microsoft.AspNetCore.Http;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class HomeController : BaseController<IUserService,User>
    {
        public IConfiguration Configuration { get; }

        private IUserService userService;
        public HomeController(IConfiguration configuration,IUserService userService)
        {
            this.Configuration = configuration;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            ViewData["Name"] = HttpContext.Session.GetString("UserName");
             return View();
        }
        public IActionResult NewIndex()
        {
            ViewData["Role"] = HttpContext.Session.GetString("Role");

            ViewData["Name"] = HttpContext.Session.GetString("UserName");
            return View();
        }
        /// <summary>
        /// 获取菜单数据
        /// </summary>
        /// <returns></returns>
        public IActionResult GetMenuData()
        {
            var menus = getMenudataList();
            return Json(menus);
        }

        public IActionResult ChangePwd()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<IActionResult> ValidateOldPwd(string pwd)
        {
            var user = await userService.GetAsync(UserId);
            if (user != null)
            {
                if (user.Password.Equals(pwd,StringComparison.OrdinalIgnoreCase))
                {
                    return ConvertSuccessResult("true");
                }
                else
                {
                    return ConvertFailResult(null,"false");
                }
            }
            else
            {
                return ConvertFailResult(null, "当前用户已经被删除！");
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<IActionResult> SavePwd(string pwd)
        {
            var model = await userService.GetAsync(UserId);
            model.Password = pwd;
            var result = await userService.UpdateAsync(model);
            return ConvertSuccessResult(result);
        }

        /// <summary>
        /// 获取菜单数据
        /// </summary>
        /// <returns></returns>
        private List<MenuModel> getMenudataList()
        {
            XDocument doc = XDocument.Load(Path.Combine(appBasePath, "Config", "menuConfig.xml"));
            var menuPs = from d in doc.Descendants("Menu_P")
                         select d;
            List<MenuModel> result = new List<MenuModel>();
            //是否启动权限控制
            //bool isNeedPemission =
            //    Configuration["permission:isNeeded"].Equals("true", StringComparison.OrdinalIgnoreCase);
            var ids = new List<string>();
            //if (isNeedPemission)
            //{
            //    var list = HttpContext.Session.Get<List<AuthResourceData>>("permission");
            //    if (list != null)
            //    {
            //        ids = list.ToList().Select(c => c.Resouce.ToLower()).ToList();
            //    }
            //}
            var currentUserRole = HttpContext.Session.GetString("Role");
           

            foreach (var xElement in menuPs)
            {
                MenuModel menuModelP = new MenuModel();
                menuModelP.Name = xElement.Attribute("Name").Value;
                menuModelP.FeatureId = xElement.Attribute("FeatureId").Value.ToLower();
                menuModelP.Url = xElement.Attribute("Url").Value;
                menuModelP.Roles = xElement.Attribute("Roles").Value
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                menuModelP.ChildrenMenus = new List<MenuModel>();
                //if (isNeedPemission) //控制顶级模块
                //{
                //    if (!ids.Contains(menuModelP.FeatureId))
                //    {
                //        continue;
                //    }
                //}

                if (SysConfig.IsNeedPermission)
                {
                    if (menuModelP.Roles.Contains(currentUserRole)==false)
                    {
                        continue;
                    }
                }

                var menuCs = from d in xElement.Descendants("Menu_C")
                    select d;
                foreach (var element in menuCs)
                {
                    MenuModel menuModelC = new MenuModel();
                    menuModelC.Name = element.Attribute("Name").Value;
                    //menuModelC.FeatureId = element.Attribute("FeatureId").Value;
                    menuModelC.Url = element.Attribute("Url").Value;
                    menuModelP.Roles = xElement.Attribute("Roles").Value
                        .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //if (isNeedPemission) //控制子模块
                    //{
                    //    if (!ids.Contains(menuModelC.FeatureId))
                    //    {
                    //        continue;
                    //    }
                    //}
                    if (SysConfig.IsNeedPermission)
                    {
                        if (menuModelP.Roles.Contains(currentUserRole) == false)
                        {
                            continue;
                        }
                    }

                    menuModelP.ChildrenMenus.Add(menuModelC);
                }
                result.Add(menuModelP);
            }
            return result;

        }

    }
}
