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

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class HomeController : BaseController<IUserService,User>
    {
        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IActionResult Index()
        {
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

            foreach (var xElement in menuPs)
            {
                MenuModel menuModelP = new MenuModel();
                menuModelP.Name = xElement.Attribute("Name").Value;
                menuModelP.FeatureId = xElement.Attribute("FeatureId").Value.ToLower();
                menuModelP.Url = xElement.Attribute("Url").Value;
                menuModelP.ChildrenMenus = new List<MenuModel>();
                //if (isNeedPemission) //控制顶级模块
                //{
                //    if (!ids.Contains(menuModelP.FeatureId))
                //    {
                //        continue;
                //    }
                //}

                var menuCs = from d in xElement.Descendants("Menu_C")
                             select d;
                foreach (var element in menuCs)
                {
                    MenuModel menuModelC = new MenuModel();
                    menuModelC.Name = element.Attribute("Name").Value;
                    //menuModelC.FeatureId = element.Attribute("FeatureId").Value;
                    menuModelC.Url = element.Attribute("Url").Value;
                    //if (isNeedPemission) //控制子模块
                    //{
                    //    if (!ids.Contains(menuModelC.FeatureId))
                    //    {
                    //        continue;
                    //    }
                    //}

                    menuModelP.ChildrenMenus.Add(menuModelC);
                }
                result.Add(menuModelP);
            }

            return result;

        }

    }
}
