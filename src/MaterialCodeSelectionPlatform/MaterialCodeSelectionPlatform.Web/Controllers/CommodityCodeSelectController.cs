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
using System.Collections;
using MaterialCodeSelectionPlatform.Utilities;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
   

    public class CommodityCodeSelectController : BaseController<ICommodityCodeService, CommodityCode>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IPartNumberService PartNumberService;
        private IComponentTypeService componentTypeService;
        private IProjectService projectService;
        private IDeviceService deviceService;
        private IUserService userService;
      
        public CommodityCodeSelectController(IHostingEnvironment hostingEnvironment,ICommodityCodeService services, IComponentTypeService componentTypeService, IPartNumberService PartNumberService, IProjectService projectService, IDeviceService deviceService, IUserService userService)
        {
            _hostingEnvironment = hostingEnvironment;
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
                    var result2 = list2.Select(c => new DropDownListItemDTO()
                        {Text = c.Code + "-" + c.Name, Value = c.Id}).ToList();
                    return Json(result2);
                case 3://物资类型
                    var list3 = await componentTypeService.GetByParentId("ParentId", parentId);
                    list3 = list3.OrderBy(c => c.Code).ToList();
                    //if (list3.Count > 0)
                    //    list3.Insert(0, new ComponentType() { Desc = "全部", Id = "-1" });

                    var result3 = list3.Select(c => new DropDownListItemDTO() { Text = c.Code + " - " + c.Desc, Value = c.Id }).ToList();

                    if (list3.Count > 0)
                        result3.Insert(0, new DropDownListItemDTO() { Text = "全部", Value = "-1" });
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
        ///  <param name="approver">审批人ID</param>
        ///   <param name="type">【0:保存】【1：发送审批人】【2：生成物料报表】</param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateReportMaterialTakeOffDetail(List<MaterialTakeOffDetail> detailList,string approver,int type)
        {
            try
            {
                var result = await Service.UpdateReportMaterialTakeOffDetail(detailList, approver,type);
                return ConvertJsonResult("更新成功", true);
            }
            catch (Exception e)
            {
                return ConvertFailResult(null, e.ToString());
            }
        }
        /// <summary>
        /// 物资汇总表查询
        /// </summary>
        /// <param name="mtoId"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetUserMaterialTakeOffList(string mtoId,  int page, int limit)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;

            MtoSearchCondition searchCondition = new MtoSearchCondition();
            searchCondition.Page = dataPage;
            searchCondition.UserId = this.UserId;
            searchCondition.MtoId = mtoId;
            var list = await Service.GetUserMaterialTakeOffList(searchCondition);
            return ConvertListResult(list, dataPage);
        }
        /// <summary>
        /// 获取用户的【物资汇总表】
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetUserMaterialTakeOff(string mtoId="")
        {
            try
            {
                var result = await Service.GetUserMaterialTakeOff(this.UserId, mtoId);
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
        public async Task<ActionResult> ReportIndex(string mtoId, string projectid, string deviceid,string revision)
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
                ViewData["revision"] = revision;
                UserSearchCondition userSearchCondition = new UserSearchCondition();
                DataPage dataPage = new DataPage();
                dataPage.PageNo = 1;
                dataPage.PageSize = int.MaxValue;
                userSearchCondition.Page = dataPage;
                //userSearchCondition.Name = "";
                userSearchCondition.Role = -1;
                userSearchCondition.Status = 0;
                var users = await userService.GetDataList(userSearchCondition);
                ViewData["users"] = users;
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
        /// <param name="type"> 【0:保存】【1：发送审批人】【2：生成物料报表】</param>
        /// <param name="revision">版本</param>
        /// <param name="projectid">项目ID</param>
        /// <param name="deviceid">装置ID</param>
        /// <param name="templatePath">模板路径</param>
        /// <returns></returns>
        public async Task<IActionResult> DownloadExcelReport(int type,string revision, string mtoId, string projectid, string deviceid, string templatePath)
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
                    if (type == 2)
                    {
                        var newPath = ExcelHelper.WriteDataTable(result, templatePath, saveFilePath);
                        var file = DownLoad(newPath);
                        return file;
                    }
                    else
                    {
                        return Json(new DataResult() { Success = false, Message = "成功" });
                    }
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
        public async Task<IActionResult> SendApprover(List<MaterialTakeOffDetail> detailList, string approver, int type, string revision, string mtoId, string projectid, string deviceid)
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
                await UpdateReportMaterialTakeOffDetail(detailList,approver,type);
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
                    var jsonDir = Directory.GetCurrentDirectory() + "\\ReportJson\\" + projectCode + "\\" + deviceCode + "\\";
                    if (!Directory.Exists(jsonDir))
                    {
                        Directory.CreateDirectory(jsonDir);
                    }
                    //项目、用户、装置、Version、Revision
                    var jsonFilePath = $"{jsonDir}{projectCode}_{deviceCode}_{revision}_{version}_{UserName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json";
                    saveJson(jsonFilePath, result);
                    return ConvertJsonResult("成功", true);
                }
                else
                {
                    return ConvertFailResult(null,"找不到记录");
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
      
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="mto"></param>
        /// <returns></returns>
        public async Task<IActionResult> ApproveMto(MaterialTakeOff mto)
        {
            try
            {
                mto.Approver = this.UserId;
                var result = await Service.ApproveMto(mto);
                return ConvertJsonResult("审批成功", true);
            }
            catch (Exception e)
            {
                return ConvertFailResult(null, e.ToString());
            }
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportData()
        {
            //var logPath = System.IO.Path.Combine(appBasePath, "Logs", DateTime.Now.ToString("yyyyMMdd"));
            var logUrl = string.Empty;
            var logs = System.IO.Path.Combine("Logs", DateTime.Now.ToString("yyyyMMdd"));
            var logPath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, logs);
           
            MaterialTakeOffDetailCSVList result = new MaterialTakeOffDetailCSVList();
            try
            {
                result.UserId = this.UserId;
                var projectId = Request.Cookies["projectIdcd"];
                var deviceId = Request.Cookies["deviceIdcd"];

                var files = Request.Form.Files;

                if (files.Count == 1)
                {
                    var file = files[0];
                    var ext = Path.GetExtension(file.FileName);
                    if (ext.ToLower() != ".zip" && ext.ToLower() != ".csv")
                    {
                        return ConvertJsonResult("只能上传【zip】或【csv】", false);
                    }
                    var filePath = SaveFile(file);

                    if (ext.ToLower() == ".csv")
                    {
                        var csv = new MaterialTakeOffDetailCSV();
                        csv.ProjectId = projectId;
                        csv.DeviceId = deviceId;
                        csv.UserId = this.UserId;
                        csv.FileName = file.FileName;
                        getCSV(csv, filePath);
                        result.CSVList.Add(csv);
                        result = await Service.ImportData(result);//数据库操作
                       
                        var fileName = $"{Path.GetFileNameWithoutExtension(filePath)}.txt";
                        var logFilePath = System.IO.Path.Combine(logPath, fileName);//绝对路径     

                        var fileNameError = $"{Path.GetFileNameWithoutExtension(filePath)}_error.txt";                          
                        var logFilePathError = System.IO.Path.Combine(logPath, fileNameError);//绝对路径


                        logUrl = System.IO.Path.Combine(logs, fileNameError);//相对路径
                        result.LogPath = logFilePath;
                      
                        foreach (var ent in result.CSVList)
                        {
                            if (!string.IsNullOrEmpty(ent.ErrorMsg))
                            {
                                result.ErrorMsg += $"#---------------------------【{DateTime.Now}】:{Path.GetFileName(ent.FileName)}----------------------------------";
                                result.ErrorMsg += $"#{ent.ErrorMsg}";
                            }
                        }
                        WriteLog(logFilePath, JsonConvert.SerializeObject(result, Formatting.Indented));
                       
                        WriteLog(logFilePathError, result.ErrorMsg);

                    }
                    if (ext.ToLower() == ".zip")
                    {
                        var folderName = Path.GetFileNameWithoutExtension(filePath);

                        result.FileName = folderName;

                        var folderPath = System.IO.Path.Combine(appBasePath, "Uploader", DateTime.Now.ToString("yyyyMMdd"), folderName);
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        ExtractToDirectory(filePath, folderPath);
                        System.IO.File.Delete(filePath);
                        #region 处理文件夹下的CSV
                        var dFiles = Directory.GetFiles(folderPath);
                        if (dFiles.Length > 0)
                        {
                            foreach (var f in dFiles)
                            {                                
                                var csv = new MaterialTakeOffDetailCSV();
                                csv.ProjectId = projectId;
                                csv.DeviceId = deviceId;
                                csv.UserId = this.UserId;
                                csv.FileName = f;
                                getCSV(csv, f);
                                result.CSVList.Add(csv);
                                // WriteLog(logPath, JsonConvert.SerializeObject(result, Formatting.Indented) + "\r\n");
                            }
                            if (result.CSVList.Count > 0)
                            {
                                result = await Service.ImportData(result);//传到数据库

                                var fileName = $"{Path.GetFileNameWithoutExtension(folderName)}.txt";
                                var logFilePath = System.IO.Path.Combine(logPath, fileName);//绝对路径    

                                var fileNameError = $"{Path.GetFileNameWithoutExtension(folderName)}_error.txt";
                                var logFilePathError = System.IO.Path.Combine(logPath, fileNameError);//绝对路径

                                logUrl = System.IO.Path.Combine(logs, fileNameError);//相对路径
                                result.LogPath = logFilePath;
                                foreach (var ent in result.CSVList)
                                {
                                    if (!string.IsNullOrEmpty(ent.ErrorMsg))
                                    {
                                        result.ErrorMsg += $"#---------------------------【{DateTime.Now}】:{Path.GetFileName(ent.FileName)}----------------------------------";
                                        result.ErrorMsg += $"#{ent.ErrorMsg}";
                                    }
                                }

                                WriteLog(logFilePath, JsonConvert.SerializeObject(result, Formatting.Indented));
                                
                                WriteLog(logFilePathError, result.ErrorMsg);
                                //foreach (var ent in result.CSVList)
                                //{
                                //    logPath = System.IO.Path.Combine(logPath, $"{Path.GetFileNameWithoutExtension(ent.FileName)}.txt");                                  
                                //    WriteLog(logPath, JsonConvert.SerializeObject(ent, Formatting.Indented) + "\r\n");
                                //}
                            }
                        }
                        #endregion
                    }                   
                    var msg = result.Success ? "成功" : $"失败：请查看日志文件";
                    return ConvertJsonResult(msg, result.Success, result,null, logUrl);
                }
                else
                {
                    return ConvertJsonResult("请选择文件,只能选择一个文件", false);
                }
            }
            catch (Exception e)
            {
                result.LogMsg+=e.ToString();                
                var fileName = $"error.txt";
                logPath = System.IO.Path.Combine(logPath, fileName);//绝对路径                   
                logUrl = System.IO.Path.Combine(logs, fileName);//相对路径
                result.LogPath = logPath;
                WriteLog(logPath, e.ToString()+result.SerializeToString());
                return ConvertJsonResult("失败", false, result, e, logUrl);
            }
        }
        private static void getCSV(MaterialTakeOffDetailCSV csv, string filePath)
        {
            var errorPath = string.Empty;
            var errorMsg = string.Empty;
            string strline;
            StreamReader mysr = new StreamReader(filePath, System.Text.Encoding.Default);
            while ((strline = mysr.ReadLine()) != null)
            {
                #region 例子
                //$B: SSF: PDMS - I:12.60:18 - Jul - 2019 21.13.04:PI: lijunlong: 1100101 - D273.0X6.35 - PN16 - FW.1 *
                //$L: 110:1100101 - D273.0X6.35 - PN16 - FW:1.6A1: 1100101 - D273.0X6.35 - PN16 - FW:::::::*
                //$S: 01:SJ04T01SP006 - A01#EFF-DW-0101:0::::*
                //$C: 250:::P0::PPPSP000731::1.112:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::P0::PPPSP000731::0.100:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::P0::PPPSP000731::0.292:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::P0::PPPSP000731::0.195:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:125::RE::PBREE002286::1:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:15::WON::POFNO003132::1:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::E90::PBEAA000620::1:1:::N: N: N:::::Y: Y:::110 *
                //$C::::::PCOM - 225::1:3:::N: N: N: HG - PN16::::Y:Y:::110 *
                //$C: 250:::FW0::PFMWN000947::1:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::FW0::PFMWN000947::1:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::FW0::PFMWN000947::1:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::FW0::PFMWN000947::1:1:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::G0::PGNFG000003::1:3:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::G0::PGNFG000003::1:3:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::G0::PGNFG000003::1:3:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::G0::PGNFG000003::1:3:::N: N: N:::::Y: Y:::110 *
                //$C::::::PLBAA001115::12:3:::N: N: N:::::Y: Y:::110 *
                //$C::::::PLBAA001115::12:3:::N: N: N:::::Y: Y:::110 *
                //$C::::::PLBAA001115::12:3:::N: N: N:::::Y: Y:::110 *
                //$C::::::PLBAA001115::12:3:::N: N: N:::::Y: Y:::110 *
                //$C: 250:::VT0::PVGAA000154::1:3:::N: N: N:::::Y: Y:::110 *

                #endregion

                var strs = strline.Split(':');
                var str0 = strs[0];//标识【$B：项目 】【$L：设备】【$C：PartNumber】
                var str1 = strs[1];//项目或设备

                if (!string.IsNullOrEmpty(str0) && str0.ToLower() == "$b")
                {
                    csv.ProjectCode = str1;
                }
                if (!string.IsNullOrEmpty(str0) && str0.ToLower() == "$l")
                {
                    csv.DeviceCode = str1;
                }
                if (!string.IsNullOrEmpty(str0) && str0.ToLower() == "$c")
                {
                    var str6 = strs[6];//PartNumber
                    var str8 = strs[8];//descignQty
                    if (!string.IsNullOrEmpty(str6) && !string.IsNullOrEmpty(str8))
                    {
                        var qty = Convert.ToDouble(str8);
                        if (!csv.PartNumberDesignQty.ContainsKey(str6))
                        {
                            csv.PartNumberDesignQty.Add(str6, qty);
                        }
                        else
                        {
                            var count = csv.PartNumberDesignQty[str6];
                            csv.PartNumberDesignQty[str6] = count + qty;
                        }
                    }
                }
            }
        }

        private void ExtractToDirectory(string zipPath, string outPath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ZipFile.ExtractToDirectory(zipPath, outPath, Encoding.GetEncoding("GB2312"));
        }
        private void WriteLog(string path, string text)
        {
           
           // var time = $"\r\n============================时间：{DateTime.Now}=======================================\r\n";
            if (!string.IsNullOrEmpty(text))
            {
                text =  text.Replace("#", "\r\n");
            }
            // var jasonString = JsonConvert.SerializeObject(result, Formatting.Indented);
            var saveDir = Path.GetDirectoryName(path);
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true, System.Text.Encoding.Default))
            {
                file.WriteLine(text);// 直接追加文件末尾，换行
            }
           
        }
    }
}