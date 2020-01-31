using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using log4net.Util;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Service;
using MaterialCodeSelectionPlatform.Utilities;
using MaterialCodeSelectionPlatform.Web.Common;
using Microsoft.AspNetCore.Http;

namespace MaterialCodeSelectionPlatform.Web.Controllers
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

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ImportData()
        {
            var files = Request.Form.Files;
            string msg = String.Empty;
            if (files.Count == 1)
            {
                var file = files[0];
                var filePath = SaveFile(file);
                var errorPath = string.Empty;
                var errorMsg = string.Empty;
                var result = importDeviceData(filePath,out errorPath,out errorMsg);
                if (result)
                {
                    return ConvertSuccessResult(null, "导入完成!");
                }
                else
                {
                    if (errorPath.IsNullOrEmpty())
                    {
                        return ConvertFailResult(null, errorMsg);
                    }
                    else
                    {
                        HttpContext.Session.SetString("ErrorDevicePath", errorPath);
                        return ConvertFailResult(null, "haveError");
                    }
                }
            }
            else
            {
                return ConvertFailResult(null, "请选择文件");
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool importDeviceData(string filePath,out string errorPath,out string errorMsg)
        {
            errorMsg = string.Empty;
            errorPath = string.Empty;
            var table = ExcelHelper.ReadExcelToDataTable(filePath);
            var errorTable = new DataTable();


            if (table == null)
            {
                errorMsg = "导入的文件内容格式错误！";
                return false;
            }

            //判断 列是是否符合模板要求

            List<string> mobColumns = new List<string>();
            mobColumns.Add("项目名称");
            mobColumns.Add("装置名称");
            mobColumns.Add("装置编码");
            mobColumns.Add("描述");
            foreach (var mobColumn in mobColumns)
            {
                errorTable.Columns.Add(mobColumn, typeof(string));
                if (table.Columns.Contains(mobColumn) == false)
                {
                    errorMsg += mobColumn+",";
                }
            }

            if (string.IsNullOrEmpty(errorMsg)==false)
            {
                errorMsg = "导入的文件缺失" + errorMsg.TrimEnd(',') + "列";
                return false;
            }

            errorTable.Columns.Add("错误信息", typeof(string));

            foreach (DataRow tableRow in table.Rows)
            {
                var errRowMsg = string.Empty;
                var projectName = tableRow["项目名称"]?.ToString();
                var deviceName = tableRow["装置名称"]?.ToString();
                var deviceCode = tableRow["装置编码"]?.ToString();
                var desc = tableRow["描述"]?.ToString();
                //验证项目是否存在

                var project = projectService.GetByParentId("Name", projectName).Result.FirstOrDefault();
                if (project == null)
                {
                    errRowMsg += "项目名称不存在";
                    DataRow dr = errorTable.NewRow();
                    dr = insertToErroTabel(dr, errRowMsg, tableRow);
                    errorTable.Rows.Add(dr);
                    continue;

                }

                var projectId = project.Id;

                //根据装置编码，判断是否存在

               var devices = Service.GetByParentId("ProjectId", projectId).Result;
               var device = devices.Where(c => c.Code.Equals(deviceCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

               if (device == null)//新增
               {
                    //判断装置名称是否有重复
                    var d = devices.Where(c => c.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
                    if (d != null)
                    {
                        errRowMsg += "存在重复的装置名称";
                        DataRow dr = errorTable.NewRow();
                        dr = insertToErroTabel(dr, errRowMsg, tableRow);
                        errorTable.Rows.Add(dr);
                        continue;
                    }
                    device = new Device();
                    device.Id = Guid.NewGuid().ToString();
                    device.Name = deviceName;
                    device.Code = deviceCode;
                    device.ProjectId = projectId;
                    device.ProjectName = projectName;
                    device.Remark = desc;
                    device.CreateTime =DateTime.Now;
                    device.CreateUserId = UserId;
                    device.Flag = 0;
                    device.Status = 0;
                    device.LastModifyUserId = UserId;
                    device.LastModifyTime =DateTime.Now;
                    var d1 = Service.SaveAsync(device).Result;

               }
               else//修改
               {
                   if (device.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase)==false)
                   {
                       device.Name = deviceName;
                       device.LastModifyUserId = UserId;
                       device.LastModifyTime =DateTime.Now;
                       var d = Service.UpdateAsync(device).Result;
                   }
               }
            }

            if (errorTable.Rows.Count > 0)
            {
                var ext = Path.GetExtension(filePath);
                errorPath = filePath.Replace(ext, "_Error" + ext);
                ExcelHelper.WriteTableToExcel(new List<DataTable>() {errorTable}, new List<string>() {"装置导入"},
                    errorPath);
                return false;
            }
            else
            {
                return true;
            }

        }

        private DataRow insertToErroTabel(DataRow dr, string errorMsg,DataRow sourceRow)
        {
            dr["装置名称"] = sourceRow["装置名称"];
            dr["项目名称"] = sourceRow["项目名称"]; 
            dr["装置编码"] = sourceRow["装置编码"]; 
            dr["描述"] = sourceRow["描述"]; 
            dr["错误信息"] = errorMsg;
            return dr;
        }


        /// <summary>
        /// 下载错误文件
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadErrorFile()
        {
            var errorPath = HttpContext.Session.GetString("ErrorDevicePath");
            return DownLoad(errorPath);
        }


        /// <summary>
        /// 下载错误文件
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadTemplateFile()
        {
            var path = System.IO.Path.Combine(appBasePath, "Templates", "Device.xlsx");
            return DownLoad(path);
        }

    }
}