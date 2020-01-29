using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net.Util;
using CommodityCodeSelectionPlatform.Domain;
using CommodityCodeSelectionPlatform.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using CommodityCodeSelectionPlatform.ManagerWeb;
using CommodityCodeSelectionPlatform.Service;

namespace CommodityCodeSelectionPlatform.Web.Controllers
{
    public class DeviceController : BaseController<IDeviceService,Device>
    {
        private IProjectService projectService;
        public DeviceController(IDeviceService deviceService,IProjectService projectService)
        {
            Service = deviceService;
            this.projectService = projectService;
        }
        public IActionResult Index(string projectId)
        {
            ViewData["projectId"] = projectId;
            return View();
        }

        public IActionResult Detail(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        public IActionResult AddOrEditPage(string id, string projectId)
        {
            ViewData["id"] = id;
            ViewData["projectId"] = projectId;
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
        public async Task<IActionResult> GetDataList(string name, string code, string projectId, int page, int limit)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;
            if (string.IsNullOrEmpty(projectId))
            {
                projectId = Guid.Empty.ToString();
            }

            DeviceSearchCondition deviceSearchCondition = new DeviceSearchCondition();
            deviceSearchCondition.Page = dataPage;
            deviceSearchCondition.Code = code;
            deviceSearchCondition.Name = name;
            deviceSearchCondition.ProjectId = projectId;
            var list = await Service.GetDataList(deviceSearchCondition);
            return ConvertListResult(list, dataPage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IActionResult> VerifData(string value,string projectId, string type, string id)
        {
            var Success = true;
            switch (type)
            {
                case "name":
                    Success = await Service.VerifNameOrCodeAsync(value, projectId, id);
                    break;
                default:
                    break;
            }
            return ConvertSuccessResult(Success);
        }

        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetProjectList()
        {
            var result =await projectService.GetListAsync();
            var list = result.Select(c => new DropDownListItemDTO() {Text = c.Name, Value = c.Id});
            return Json(list);
        }


    }
}