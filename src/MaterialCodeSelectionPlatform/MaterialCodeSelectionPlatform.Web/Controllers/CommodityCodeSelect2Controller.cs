﻿using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Service;
using MaterialCodeSelectionPlatform.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using log4net;
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
        private IChangeHistoryService changeHistoryService;
        private ILog log = LogHelper.GetLogger<CommodityCodeSelect2Controller>();
        public CommodityCodeSelect2Controller(ICommodityCodeService services, IComponentTypeService componentTypeService, IPartNumberService PartNumberService, IProjectService projectService, IDeviceService deviceService, IChangeHistoryService changeHistoryService)
        {
            this.Service = services;
            this.PartNumberService = PartNumberService;
            this.componentTypeService = componentTypeService;
            this.projectService = projectService;
            this.deviceService = deviceService;
            this.changeHistoryService = changeHistoryService;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult ChangeHistoryIndex(string materialTakeOffId)
        {
            ViewData["materialTakeOffId"] = materialTakeOffId;
            return View();
        }

        /// <summary>
        /// 查询变更历史
        /// </summary>
        /// <param name="materialTakeOffId"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetChangeDataList(string materialTakeOffId, int page, int limit)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;


            ChangeHistorySearchCondition changeHistorySearchCondition = new ChangeHistorySearchCondition();
            changeHistorySearchCondition.Page = dataPage;
            changeHistorySearchCondition.MaterialTakeOffId = materialTakeOffId;
            var list = await changeHistoryService.GetDataList(changeHistorySearchCondition);
            return ConvertListResult(list, dataPage);
        }

        /// <summary>
        /// 导出变更历史
        /// </summary>
        /// <param name="materialTakeOffId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportChangeData(string materialTakeOffId)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = 1;
            dataPage.PageSize = 10000;


            ChangeHistorySearchCondition changeHistorySearchCondition = new ChangeHistorySearchCondition();
            changeHistorySearchCondition.Page = dataPage;
            changeHistorySearchCondition.MaterialTakeOffId = materialTakeOffId;
            var list = await changeHistoryService.GetDataList(changeHistorySearchCondition);

            DataTable table = new DataTable();
            table.Columns.Add("变更列", typeof(string));
            table.Columns.Add("变更前内容", typeof(string));
            table.Columns.Add("变更后内容", typeof(string));
            table.Columns.Add("变更日期", typeof(string));

            foreach (var changeHistory in list)
            {
                DataRow dr = table.NewRow();
                dr["变更列"] = changeHistory.ColumnName;
                dr["变更前内容"] = changeHistory.Old;
                dr["变更后内容"] = changeHistory.New;
                dr["变更日期"] = changeHistory.ChangeDate;
                table.Rows.Add(dr);
            }

            var filePath = Path.Combine(appBasePath, "TempFiles",
                "ChangeDat_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

            ExcelHelper.ExportData(table, filePath);
            return DownLoad(filePath);
        }


        public async Task<IActionResult> IsNeedGetChangeData(string materialTakeOffId)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = 1;
            dataPage.PageSize = 100;


            ChangeHistorySearchCondition changeHistorySearchCondition = new ChangeHistorySearchCondition();
            changeHistorySearchCondition.Page = dataPage;
            changeHistorySearchCondition.MaterialTakeOffId = materialTakeOffId;
            var list = await changeHistoryService.GetDataList(changeHistorySearchCondition);
            if (list.Count > 0)
            {
                return ConvertSuccessResult(true);
            }
            else
            {
                return ConvertSuccessResult(false);
            }
          
        }

        public async Task<IActionResult> DeleteChangeData(string id)
        {
            List<string> ids = id.Split(new []{ ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var id1 in ids)
            {
                await changeHistoryService.DeleteByIdAsync(id1);
            }
            return ConvertSuccessResult(true);
        }


        /// <summary>
        /// 根据物资编码描述获取物资类型
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCompenetTypeByMCDesc(string desc, string catalogId)
        {
            log.Debug($"开始搜索 {desc} 的数据,编码库：{catalogId}");
            Stopwatch watch = new Stopwatch();
            watch.Start();

            desc = desc.Replace(@"\\\", "PRPRPR").Replace("\\", "").Replace("PRPRPR", @"\\\");
            var result = await componentTypeService.GetByCommodityCodeDesc(catalogId, desc);

            result = result.OrderByDescending(c => c.ComponentTypeCode).ToList();
            log.Debug($"完成搜索 {desc} 的数据,编码库：{catalogId},耗时：{watch.ElapsedMilliseconds}毫秒");
            return ConvertSuccessResult(result);
        }

        /// <summary>
        /// 获取当前平级的物资类型
        /// </summary>
        /// <param name="compenentTypeId"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetCompenetById(string compenentTypeId,string catalogId)
        {

            var currentCompType = await componentTypeService.GetAsync(compenentTypeId);
            if (currentCompType == null)
            {
                //如果传过来的是编码库Id
                var result = await componentTypeService.GetByColumnValuess("CatalogId,ParentId", compenentTypeId+","+Guid.Empty.ToString());

                result = result.OrderBy(c => c.Code).ToList();
                var list = result.Select(c => new DropDownListItemDTO() { Text = c.Code + " - "+ c.Desc, Value = c.Id }).ToList();
                list.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
                return Json(list);
            }
            else
            {
                if (catalogId.IsNotNullAndNotEmpty())
                {
                    var result = await componentTypeService.GetByColumnValuess("ParentId,CatalogId", currentCompType.ParentId + ","+ catalogId);

                    var list = result.Select(c => new DropDownListItemDTO() { Text = c.Code + " - " + c.Desc, Value = c.Id }).ToList();
                    list.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
                    return Json(list);
                }
                else
                {
                    var result = await componentTypeService.GetByParentId("ParentId", currentCompType.ParentId);

                    var list = result.Select(c => new DropDownListItemDTO() { Text = c.Code + " - " + c.Desc, Value = c.Id }).ToList();
                    list.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
                    return Json(list);
                }
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

        public async Task<IActionResult> GetAttributeByTypeId(string compenentTypeId,string userInputText)
        {
            var list = await componentTypeService.GetAttributeByCompenetType(compenentTypeId, userInputText);
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
            if(inputText!= null)
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