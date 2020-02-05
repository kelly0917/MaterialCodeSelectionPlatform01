using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Service;
using MaterialCodeSelectionPlatform.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class CommodityCodeSelect2Controller : BaseController<ICommodityCodeService, CommodityCode>
    {
        private IPartNumberService PartNumberService;
        private IComponentTypeService componentTypeService;
        private IProjectService projectService;
        private IDeviceService deviceService;
        public CommodityCodeSelect2Controller(ICommodityCodeService services, IComponentTypeService componentTypeService, IPartNumberService PartNumberService,IProjectService projectService,IDeviceService deviceService)
        {
            this.Service = services;
            this.PartNumberService = PartNumberService;
            this.componentTypeService = componentTypeService;
            this.projectService = projectService;
            this.deviceService = deviceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据物资编码描述获取物资类型
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCompenetTypeByMCDesc(string desc,string projectId)
        {
              var result =await componentTypeService.GetByCommodityCodeDesc(projectId, desc);

              result = result.OrderByDescending(c => c.Count).Take(15).ToList();
            return ConvertSuccessResult(result);
        }



    }
}