using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Service;
using MaterialCodeSelectionPlatform.Web.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MaterialCodeSelectionPlatform.Domain.DTO;

namespace MaterialCodeSelectionPlatform.Web.Controllers
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
            ViewData["UserId"] = this.UserId;
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
                    var projectList = list1.ToList();
                    if (projectList.Count > 0)
                        projectList.Insert(0, new Project() { Name = "全部", Id = "-1" });
                    var result1 = projectList.Select(c => new DropDownListItemDTO() { Text = c.Name, Value = c.Id }).ToList();
                    return Json(result1);
                case 2://装置
                    var list2 = await deviceService.GetByParentId("ProjectId", parentId);
                    var deviceList = list2.ToList();
                    if (deviceList.Count > 0)
                        deviceList.Insert(0, new Device() { Name = "全部", Id = "-1" });
                    var result2 = list2.Select(c => new DropDownListItemDTO() { Text = c.Name, Value = c.Id }).ToList();
                    return Json(result2);
                case 3://物资类型
                    var list3 = await componentTypeService.GetByParentId("ParentId",parentId);
                    if(list3.Count>0)
                        list3.Insert(0,new ComponentType(){ Desc = "全部",Id="-1"});
                    var result3 = list3.Select(c => new DropDownListItemDTO() { Text = c.Desc, Value = c.Id }).ToList();
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
        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="code">物资编码</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页显示的记录数</param>
        /// <param name="componentTypeId">物资类型ID</param>
        /// <returns></returns>
        public async Task<IActionResult> GetCommodityCodeDataList(string code, int page, int limit,string componentTypeId,List<AttributeModel> compenentCodeIds)
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
            condition.ComponentTypeId = componentTypeId;
            condition.CompenetAttributes = compenentCodeIds;
            condition.Page = dataPage;
            condition.Code = code;          
            var list = await this.Service.GetCommodityCodeDataList(condition);
            return ConvertListResult(list, dataPage);
        }
        /// <summary>
        ///【 物资编码】属性
        /// </summary>
        /// <param name="id">物资编码Id</param>
        /// <returns></returns>
        public async Task<ActionResult> AttrList(string id)
        {           
            var list = await this.Service.GetAttributeList(id);
            return View(list);
        }
        /// <summary>
        ///【 物资类型】属性
        /// </summary>
        /// <param name="id">物资编码Id</param>
        /// <returns></returns>
        public async Task<ActionResult> GetTypeAttributeList(string id)
        {
            var list = await this.Service.GetComponentTypeAttributeList(id);
            return Json(list);
        }
        /// <summary>
        /// 选择【物资编码】的采购码
        /// </summary>
        /// <param name="commodityCodeId">物资编码Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="deviceId">装置Id</param>
        /// <returns></returns>
        public async Task<ActionResult> CommodityCodePartNumberList(string commodityCodeId, string userId,string projectId, string deviceId)
        {
            ViewData["commodityCodeId"] = commodityCodeId;
            ViewData["projectId"] = projectId;
            ViewData["deviceId"] = deviceId;
            var list = await this.Service.GetCommodityCodePartNumberList(commodityCodeId, userId, projectId, deviceId);
            return View(list);
        }
        /// <summary>
        /// 保存【物资汇总明细表】
        /// </summary>
        /// <param name="list">采购码</param>
        /// <param name="commodityCodeId">物资编码Id</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="deviceId">装置ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveMaterialTakeOffDetail(List<PartNumberDto> list,string commodityCodeId,string projectId, string deviceId)
        {
            PartNumberCondition condition = new PartNumberCondition();
            condition.UserId = this.UserId;
            condition.ProjectId = projectId;
            condition.DeviceId = deviceId;
            condition.CommodityCodeId = commodityCodeId;
            condition.PartNumberDtoList = list;
            if (condition.PartNumberDtoList != null && condition.PartNumberDtoList.Count > 0)
            {
                foreach (var ent in condition.PartNumberDtoList)
                {
                    ent.CreateUserId = this.UserId;
                    ent.LastModifyUserId = this.UserId;
                    ent.CreateTime = DateTime.Now;
                    ent.LastModifyTime = DateTime.Now;
                }
            }
            try
            {
                var result = await Service.SaveMaterialTakeOffDetail(condition);
                return ConvertJsonResult("更新成功",true);
            }
            catch (Exception e)
            {
                return ConvertFailResult(null, e.ToString());
            }
        }
        /// <summary>
        /// 获取用户的【物资汇总表】
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetUserMaterialTakeOff()
        {          
            try
            {
                var result = await Service.GetUserMaterialTakeOff(this.UserId);
                return ConvertJsonResult("成功", true, result);
            }
            catch (Exception e)
            {
                return ConvertJsonResult("失败", false,e.Message,e);
            }
        }
        /// <summary>
        /// 获取用户的物料表
        /// </summary>
        /// <param name="mtoId">物资汇总表Id</param>
        /// <param name="projectid">项目Id</param>
        /// <param name="deviceid">装置Id</param>
        /// <returns></returns>
        public async Task<ActionResult> ReportIndex(string mtoId, string projectid, string deviceid)
        {
            try
            {
                var dirPath = Directory.GetCurrentDirectory() + "\\ReportTemplates\\";
                NameValueCollection fileList = new NameValueCollection();
                getTemplate(dirPath,ref fileList);
                ViewData["mtoId"] = mtoId;
                ViewData["fileList"] = fileList;
                ViewData["projectid"] = projectid;
                ViewData["deviceid"] = deviceid;
                var result = await Service.GetUserMaterialTakeReport(mtoId,this.UserId, projectid, deviceid,0);
                return View(result);
            }
            catch (Exception e)
            {
                return View();
            }
        }
        /// <summary>
        /// 导出物料表
        /// </summary>
        /// <param name="projectid">项目ID</param>
        /// <param name="deviceid">装置ID</param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadExcelReport(string mtoId, string projectid, string deviceid,string templatePath)
        {
            try
            {
                //C:\工作\GIT\src\MaterialCodeSelectionPlatform\MaterialCodeSelectionPlatform.Web\ReportTemplates\管道综合材料表\管道综合材料表_ENG.xlsx
                
                templatePath = HttpUtility.UrlDecode(templatePath);
                var saveDir= Directory.GetCurrentDirectory() + "\\ReportTemplates\\Download\\";
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                var excelName = Path.GetFileNameWithoutExtension(templatePath);
                var result = await Service.GetUserMaterialTakeReport(mtoId, this.UserId, projectid, deviceid,1);
                var dirPath = Path.GetDirectoryName(templatePath);
                var filePath = dirPath + $"{excelName}.xlsx";
                #region 删除上次生成的EXCEL文件
                var files = Directory.GetFiles(saveDir)?.ToList();
                if (files != null && files.Count > 0)
                {
                    foreach (var ent in files)
                    {
                        System.IO.File.Delete(ent);
                    }
                }
                #endregion
                var saveFilePath = saveDir + excelName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                var newPath =ExcelHelper.WriteDataTable(result, templatePath, saveFilePath);
                var file= DownLoad(newPath);               
                return file;
            }
            catch (Exception e)
            {
                return Json(new DataResult() { Success = false, Message = e.Message });
            }
        }
        private void getTemplate(string dirPath,ref NameValueCollection fileList)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            var files = dirInfo.GetFiles().Where(c => Path.GetExtension(c.FullName)?.ToLower() == ".xlsx").ToList();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var name = $"{dirInfo.Name}\\{file.Name}";
                    fileList.Add(name,file.FullName);
                }
            }
            var dirList = dirInfo.GetDirectories().Where(c=>c.Name.ToLower()!= "download").ToList();
            if (dirList != null && dirList.Count > 0)
            {
                foreach (var dir in dirList)
                {
                    getTemplate(dir.FullName,ref fileList);
                }
            }
        }
    }
}