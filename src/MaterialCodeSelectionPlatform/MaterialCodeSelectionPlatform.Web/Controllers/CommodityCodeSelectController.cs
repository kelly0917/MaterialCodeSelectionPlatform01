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
using Newtonsoft.Json;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class CommodityCodeSelectController : BaseController<ICommodityCodeService, CommodityCode>
    {
        private IPartNumberService PartNumberService;
        private IComponentTypeService componentTypeService;
        private IProjectService projectService;
        private IDeviceService deviceService;
        private IUserService userService;
        public CommodityCodeSelectController(ICommodityCodeService services, IComponentTypeService componentTypeService, IPartNumberService PartNumberService, IProjectService projectService, IDeviceService deviceService, IUserService userService)
        {
            this.Service = services;
            this.PartNumberService = PartNumberService;
            this.componentTypeService = componentTypeService;
            this.projectService = projectService;
            this.deviceService = deviceService;
            this.userService = userService;
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
        public async Task<IActionResult> GetDropDownData(int type, string parentId)
        {
            switch (type)
            {
                case 1://项目
                    var list1 = await userService.GetRightProjects(UserId);
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
                    var list3 = await componentTypeService.GetByParentId("ParentId", parentId);
                    if (list3.Count > 0)
                        list3.Insert(0, new ComponentType() { Desc = "全部", Id = "-1" });
                    var result3 = list3.Select(c => new DropDownListItemDTO() { Text = c.Desc +" - " + c.Code, Value = c.Id }).ToList();
                    return Json(result3);
                case 4://编码库
                    var catlogs = await projectService.GetRightCatalogs(parentId);
                    catlogs = catlogs.OrderBy(c => c.Description).ToList();
                    var result4 = catlogs.Select(c => new DropDownListItemDTO() { Text = c.Description, Value = c.Id });
                    return Json(result4);
                case 5://物资类型 根据编码库获取
                    list3 = await componentTypeService.GetByParentId("CatalogId", parentId);
                    if (list3.Count > 0)
                        list3.Insert(0, new ComponentType() { Desc = "全部", Id = "-1" });
                    result3 = list3.Select(c => new DropDownListItemDTO() { Text = c.Desc, Value = c.Id }).ToList();
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
        public async Task<ActionResult> CommodityCodePartNumberList(string commodityCodeId, string userId, string projectId, string deviceId)
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
        public async Task<IActionResult> SaveMaterialTakeOffDetail(List<PartNumberDto> list, string commodityCodeId, string projectId, string deviceId)
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
                return ConvertJsonResult("更新成功", true);
            }
            catch (Exception e)
            {
                return ConvertFailResult(null, e.ToString());
            }
        }
        /// <summary>
        /// 更新:物料报表【物资汇总明细表】数量
        /// </summary>
        /// <param name="detailList">MaterialTakeOffDetail集合</param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateReportMaterialTakeOffDetail(List<MaterialTakeOffDetail> detailList)
        {
            try
            {
                var result = await Service.UpdateReportMaterialTakeOffDetail(detailList);
                return ConvertJsonResult("更新成功", true);
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
                return ConvertJsonResult("失败", false, e.Message, e);
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
                getTemplate(dirPath, ref fileList);
                ViewData["mtoId"] = mtoId;
                ViewData["fileList"] = fileList;
                ViewData["projectid"] = projectid;
                ViewData["deviceid"] = deviceid;
                var result = await Service.GetUserMaterialTakeReport(mtoId, "", this.UserId, projectid, deviceid, 0);
                return View(result);
            }
            catch (Exception e)
            {
                return View();
            }
        }
        public async Task<ActionResult> CopyIndex(string projectId, string deviceId)
        {
            try
            {
                ViewData["projectId"] = projectId;
                ViewData["deviceId"] = deviceId;
                var result = await Service.GetUserMaterialTakeOff("");
                if (result != null && result.Count >= 0)
                {
                    var own = result.Where(c => c.ProjectId == projectId && c.DeviceId == deviceId && c.CreateUserId == this.UserId).FirstOrDefault();//排除自己当前的项目与装置
                    if (own != null)
                    {
                        result.Remove(own);
                    }
                }
                return View(result);
            }
            catch (Exception e)
            {
                return View();
            }
        }
        /// <summary>
        /// 拷贝
        /// </summary>
        /// <param name="mtoId"></param>
        /// <param name="type">【0：追加拷贝】【1：覆盖拷贝】</param>
        /// <returns></returns>
        public async Task<IActionResult> CopyMaterialTakeOff(string mtoId, string projectId, string deviceId, int type)
        {
            try
            {
                var n = await Service.CopyMaterialTakeOff(mtoId, this.UserId, projectId, deviceId, type);
                if (n > 0)
                {
                    return ConvertSuccessResult(null, "删除成功");
                }
                else
                {
                    return ConvertFailResult(null, "删除失败");
                }
            }
            catch (Exception e)
            {
                return Json(new DataResult() { Success = false, Message = e.Message });
            }
        }
        /// <summary>
        /// 导出物料表
        /// </summary>
        /// <param name="revision">版本</param>
        /// <param name="projectid">项目ID</param>
        /// <param name="deviceid">装置ID</param>
        /// <param name="templatePath">模板路径</param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadExcelReport(string revision, string mtoId, string projectid, string deviceid, string templatePath)
        {
            try
            {
                //C:\工作\GIT\src\MaterialCodeSelectionPlatform\MaterialCodeSelectionPlatform.Web\ReportTemplates\管道综合材料表\管道综合材料表_ENG.xlsx
                //保存到ReportDownload\$ProjectCode$\$Device$ \ 目录下
                //文件名希望调整为XXXXXXX_$Project$_$Device$_$Revision$_yyyyMMddhhmmss.xlsx。其中
                //XXXX为原来的文件名
                //$Project$为当前MTO的Project.Code
                //$Device$为当前MTO的Device.Code
                //$Revision$为手动输入的Revision字段
                //yyyyMMddhhmmss为时间戳
                templatePath = HttpUtility.UrlDecode(templatePath);

                var excelName = Path.GetFileNameWithoutExtension(templatePath);
                var result = await Service.GetUserMaterialTakeReport(mtoId, revision, this.UserId, projectid, deviceid, 1);
                if (result != null && result.Count > 0)
                {
                    result.ForEach(a =>
                    a.PartNumberReportDetailList = a.PartNumberReportDetailList?.OrderBy(c => c.T_Code).ThenBy(c => c.C_Code).ThenBy(c => c.P_Code).ToList()
                    );

                    var ent = result.FirstOrDefault();
                    var projectCode = ent.ProjectCode;
                    var deviceCode = ent.DeviceCode;
                    var version = ent.Version;
                    var saveDir = Directory.GetCurrentDirectory() + "\\ReportDownload\\" + projectCode + "\\" + deviceCode + "\\";
                    var jsonDir = Directory.GetCurrentDirectory() + "\\ReportJson\\" + projectCode + "\\" + deviceCode + "\\";
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }
                    //项目、用户、装置、Version、Revision
                    var saveFilePath = $"{saveDir}{excelName}_{projectCode}_{deviceCode}_{revision}_{version}_{UserName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    var jsonFilePath = $"{jsonDir}{excelName}_{projectCode}_{deviceCode}_{revision}_{version}_{UserName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
                    saveJson(jsonFilePath, result);
                    var newPath = ExcelHelper.WriteDataTable(result, templatePath, saveFilePath);
                    var file = DownLoad(newPath);
                    return file;
                }
                else
                {
                    return DownLoad(templatePath); ;
                }

            }
            catch (Exception e)
            {
                return Json(new DataResult() { Success = false, Message = e.Message });
            }
        }
        private void saveJson(string path, List<PartNumberReport> result)
        {

            var jasonString = JsonConvert.SerializeObject(result, Formatting.Indented);
            var saveDir = Path.GetDirectoryName(path);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, System.Text.Encoding.Default);
            sw.Write(jasonString);
            sw.Close();
            sw.Dispose();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<IActionResult> DeleteById(string id)
        {
            try
            {
                var n = await Service.DeleteById(id);
                if (n > 0)
                {
                    return ConvertSuccessResult(null, "删除成功");
                }
                else
                {
                    return ConvertFailResult(null, "删除失败");
                }
            }
            catch (Exception e)
            {
                return Json(new DataResult() { Success = false, Message = e.Message });
            }
        }
        private void getTemplate(string dirPath, ref NameValueCollection fileList)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            var files = dirInfo.GetFiles().Where(c => Path.GetExtension(c.FullName)?.ToLower() == ".xlsx").ToList();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var name = $"{dirInfo.Name}\\{file.Name}";
                    fileList.Add(name, file.FullName);
                }
            }
            var dirList = dirInfo.GetDirectories().Where(c => c.Name.ToLower() != "download").ToList();
            if (dirList != null && dirList.Count > 0)
            {
                foreach (var dir in dirList)
                {
                    getTemplate(dir.FullName, ref fileList);
                }
            }
        }
    }
}