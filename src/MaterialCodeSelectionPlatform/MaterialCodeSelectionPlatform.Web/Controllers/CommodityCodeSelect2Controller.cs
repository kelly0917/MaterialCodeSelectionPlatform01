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
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Utilities;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class CommodityCodeSelect2Controller : BaseController<ICommodityCodeService, CommodityCode>
    {
        private IPartNumberService PartNumberService;
        private IComponentTypeService componentTypeService;
        private IProjectService projectService;
        private IDeviceService deviceService;
        public CommodityCodeSelect2Controller(ICommodityCodeService services, IComponentTypeService componentTypeService, IPartNumberService PartNumberService, IProjectService projectService, IDeviceService deviceService)
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
        public async Task<IActionResult> GetCompenetTypeByMCDesc(string desc, string catalogId)
        {
            desc = desc.Replace(@"\\\", "PRPRPR").Replace("\\", "").Replace("PRPRPR", @"\\\");
            var result = await componentTypeService.GetByCommodityCodeDesc(catalogId, desc);

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
                //如果传过来的是编码库Id
                var result = await componentTypeService.GetByColumnValuess("CatalogId,ParentId", compenentTypeId+","+Guid.Empty.ToString());

                var list = result.Select(c => new DropDownListItemDTO() { Text = c.Desc, Value = c.Id }).ToList();
                list.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
                return Json(list);
            }
            else
            {
                var result = await componentTypeService.GetByParentId("ParentId", currentCompType.ParentId);

                var list = result.Select(c => new DropDownListItemDTO() { Text = c.Desc, Value = c.Id }).ToList();
                list.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
                return Json(list);
            }
           

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
                //如果传过来的是编码库Id
                var result = await componentTypeService.GetByColumnValuess("CatalogId,ParentId", compenentTypeId + "," + Guid.Empty.ToString());

                var list = result.Select(c => new DropDownListItemDTO() { Text = c.Desc, Value = c.Id }).ToList();
                list.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
                return Json(list);
         
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

        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="code">物资编码</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页显示的记录数</param>
        /// <param name="componentTypeId">物资类型ID</param>
        /// <param name="orderBy">排序列明</param>
        /// <param name="orderByType">0升序，1降序</param>
        /// <returns></returns>
        public async Task<IActionResult> GetCommodityCodeDataList(string inputText, string catalogId, int page, int limit, string componentTypeId, List<AttributeModel> compenentCodeIds, string orderBy, int orderByType = 0)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;
            //attrName = HttpUtility.UrlDecode(attrName);
            //attrValue = HttpUtility.UrlDecode(attrValue);
            CommodityCodeSerachCondition condition = new CommodityCodeSerachCondition();
            //condition.AttrName = attrName;
            //if (!string.IsNullOrEmpty(attrValue))
            //{
            //    condition.AttrValue = attrValue.Split(',').ToList();
            //}

            inputText = inputText.Replace(@"\\\", "PRPRPR").Replace("\\", "").Replace("PRPRPR", @"\\\");
            condition.ComponentTypeId = componentTypeId;
            condition.CompenetAttributes = compenentCodeIds;
            condition.Page = dataPage;
            condition.InputText = inputText;
            condition.OrderBy = orderBy;
            condition.OrderByType = orderByType;
            condition.CatelogId = catalogId;
            var list = await this.Service.GetCommodityCodeDataList(condition);
            return ConvertListResult(list, dataPage);
        }

    }
}