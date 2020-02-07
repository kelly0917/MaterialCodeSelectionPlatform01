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
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MaterialCodeSelectionPlatform.Utilities;

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

        /// <summary>
        /// 获取当前平级的物资类型
        /// </summary>
        /// <param name="compenentTypeId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCompenetById(string compenentTypeId)
        {
            var currentCompType = await componentTypeService.GetAsync(compenentTypeId);
            if (currentCompType == null)
            {
                return ConvertFailResultStr(null, "获取数据失败");
            }

            var result = await componentTypeService.GetByParentId("ParentId", currentCompType.ParentId);

            var list = result.Select(c => new DropDownListItemDTO() { Text = c.Desc, Value = c.Id }).ToList();
            list.Insert(0,new DropDownListItemDTO(){Text = "全部",Value = "-1"});
            return Json(list);
        }

        /// <summary>
        /// 获取物资编码的父Id
        /// </summary>
        /// <param name="compenentTypeId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetParentCompenetById(string compenentTypeId)
        {
            var currentCompType = await componentTypeService.GetAsync(compenentTypeId);
            if (currentCompType == null)
            {
                return ConvertFailResultStr(null, "获取数据失败");
            }

            if (currentCompType.ParentId.IsNullOrEmpty() || currentCompType.ParentId == Guid.Empty.ToString())
            {
                return ConvertFailResultStr(null, "已经是顶级物资类型");
            }
            return ConvertSuccessResult(currentCompType.ParentId);
        }

        public async Task<IActionResult> GetAttributeByTypeId(string compenentTypeId)
        {
            var list = await componentTypeService.GetAttributeByCompenetType(compenentTypeId);
            return ConvertSuccessResult(list);
        }



    }
}