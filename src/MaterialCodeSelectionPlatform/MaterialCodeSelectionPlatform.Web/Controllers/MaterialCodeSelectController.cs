﻿using System;
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
    public class CommodityCodeSelectController : BaseController<ICommodityCodeService, CommodityCode>
    {
        private IPartNumberService PartNumberService;
        private IComponentTypeService componentTypeService;
        private IProjectService projectService;
        private IDeviceService deviceService;
        public CommodityCodeSelectController(ICommodityCodeService services, IComponentTypeService componentTypeService, IPartNumberService PartNumberService,IProjectService projectService,IDeviceService deviceService)
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
        /// 获取所有物资类型
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetDropDownData(int type,string parentId)
        {
            switch (type)
            {
                case 1://项目
                    var list1 = await projectService.GetListAsync();
                    var result1 = list1.Select(c => new DropDownListItemDTO() { Text = c.Name, Value = c.Id }).ToList();
                    return Json(result1);
                case 2://装置
                    var list2 = await deviceService.GetByParentId("ProjectId", parentId);
                    var result2 = list2.Select(c => new DropDownListItemDTO() { Text = c.Name, Value = c.Id }).ToList();
                    return Json(result2);
                case 3://物资类型
                    var list3 = await componentTypeService.GetByParentId("ParentId",parentId);
                    if(list3.Count>0)
                        list3.Insert(0,new ComponentType(){Name = "全部",Id="-1"});
                    var result3 = list3.Select(c => new DropDownListItemDTO() { Text = c.Name, Value = c.Id }).ToList();
                    return Json(result3);
                //case 4://物资编码
                //    var list4 = await Service.GetByParentId("ComponentTypeId",parentId);
                //    var result4 = list4.Select(c => new DropDownListItemDTO() { Text = c.Code, Value = c.Id });
                //    return Json(result4);
                //case 5://采购编码
                //    var list5 = await PartNumberService.GetByParentId("CommodityCodeId", parentId);
                //    var result5 = list5.Select(c => new DropDownListItemDTO() { Text = c.Code, Value = c.Id });
                //    return Json(result5);
                default:
                    return Content("error");
            }
        }

        /// <summary>
        /// 查看属性
        /// </summary>
        /// <returns></returns>
        public IActionResult LookAttribute(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        public IActionResult SelectData()
        {
            return View();
        }

        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAttributeByMCId(string id)
        {
            var list = await Service.GetAttributeById(id, LanguageType);

            DataPage dataPage = new DataPage();
            dataPage.PageNo = 1;
            dataPage.PageSize = 10000;
            
            return ConvertListResult(list, dataPage);
        }

        /// <summary>
        /// 获取物资编码下的采购码，以及采购码属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPartNumberById(string id)
        {
            return null;
        }

    }
}