using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Domain;
using CommodityCodeSelectionPlatform.Domain.Entities;
using CommodityCodeSelectionPlatform.ManagerWeb;
using CommodityCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Mvc;

namespace CommodityCodeSelectionPlatform.Web.Controllers
{
    public class ProjectController : BaseController<IProjectService, Project>
    {

        public ProjectController(IProjectService projectService)
        {
            Service = projectService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        public IActionResult AddOrEditPage(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDataList(string name, string code, int status, int page, int limit)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;

            ProjectSearchCondition projectSearchCondition = new ProjectSearchCondition();
            projectSearchCondition.Page = dataPage;
            projectSearchCondition.Code = code;
            projectSearchCondition.Name = name;
            projectSearchCondition.Status = status;
            var list = await Service.GetDataList(projectSearchCondition);
            return ConvertListResult(list, dataPage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IActionResult> VerifData(string value, string type, string id)
        {
            var Success = true;
            switch (type)
            {
                case "name":
                    Success = await Service.VerifNameOrCodeAsync(value, id);
                    break;
                default:
                    break;
            }
            return ConvertSuccessResult(Success);
        }

   

    }
}