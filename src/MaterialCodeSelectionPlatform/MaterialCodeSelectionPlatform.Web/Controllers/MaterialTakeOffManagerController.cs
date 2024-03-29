﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Mvc;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class MaterialTakeOffManagerController : BaseController<IMaterialTakeOffDetailService, MaterialTakeOffDetail>
    {
        private IMaterialTakeOffService materialTakeOffService;
        public MaterialTakeOffManagerController(IMaterialTakeOffDetailService materialTakeOffDetailService, IMaterialTakeOffService materialTakeOffService)
        {
            this.Service = materialTakeOffDetailService;
            this.materialTakeOffService = materialTakeOffService;
        }


        public async Task<IActionResult> Index()
        {
            var mtoId = Request.Cookies["mtoId"];

            ViewData["mtoId"] = mtoId;
            var model = await materialTakeOffService.GetAsync(mtoId);
            if (model!=null && model.Approver != null && model.Approver.Equals(UserId, StringComparison.OrdinalIgnoreCase))
            {
                if (model.CheckStatus.HasValue && model.CheckStatus.Value == 2)
                {

                }
                else
                {
                    ViewData["isApprover"] = "true";
                }


            }
            return View();
        }

        public IActionResult UpdatePage(string detailIds)
        {
            ViewData["detailIds"] = detailIds;
            return View();
        }

        public IActionResult ApprovePage(string mtoId)
        {
            ViewData["mtoId"] = mtoId;
            return View();
        }



        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="deviceId"></param>
        /// <param name="componentTypeCode"></param>
        /// <param name="componentTypeDesc"></param>
        /// <param name="partNumberCNDesc"></param>
        /// <param name="partNumberENDesc"></param>
        /// <param name="partNumberRUDesc"></param>
        /// <param name="commodityCode"></param>
        /// <param name="partNumberCode"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDataList(string projectId, string deviceId, string componentTypeCode, string componentTypeDesc, string partNumberCNDesc, string partNumberENDesc, string partNumberRUDesc, string commodityCode, string partNumberCode, int page, int limit, string orderBy, int orderByType = 0)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;
            MaterialTakeOffDetailSearchCondition materialTakeOffDetailSearchCondition = new MaterialTakeOffDetailSearchCondition();
            materialTakeOffDetailSearchCondition.CommodityCode = commodityCode;
            materialTakeOffDetailSearchCondition.ComponentTypeCode = componentTypeCode;
            materialTakeOffDetailSearchCondition.ComponentTypeDesc = componentTypeDesc;
            materialTakeOffDetailSearchCondition.DeviceId = deviceId;
            materialTakeOffDetailSearchCondition.PartNumberCNDesc = partNumberCNDesc;
            materialTakeOffDetailSearchCondition.PartNumberCode = partNumberCode;
            materialTakeOffDetailSearchCondition.PartNumberENDesc = partNumberENDesc;
            materialTakeOffDetailSearchCondition.PartNumberRUDesc = partNumberRUDesc;
            materialTakeOffDetailSearchCondition.ProjectId = projectId;
            materialTakeOffDetailSearchCondition.OrderBy = orderBy;
            materialTakeOffDetailSearchCondition.OrderByType = orderByType;
            materialTakeOffDetailSearchCondition.Page = dataPage;

            var data = await Service.GetManagerList(materialTakeOffDetailSearchCondition);

            return ConvertListResult(data, dataPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="detailIds"></param>
        /// <param name="allowance"></param>
        /// <param name="roundUpDigit"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveData(string detailIds, double allowance, int roundUpDigit)
        {
            List<string> detailList =
                detailIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<MaterialTakeOffDetail> datas = new List<MaterialTakeOffDetail>();
            foreach (var detailId in detailList)
            {
                var data = await Service.GetAsync(detailId);
                data.Allowance = allowance / 100;
                data.RoundUpDigit = roundUpDigit;
                datas.Add(data);
            }
            await Service.UpdateMaterialTakeOffDetail(datas);
            return ConvertSuccessResult("success");
        }
    }
}