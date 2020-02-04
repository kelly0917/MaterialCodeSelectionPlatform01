using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Mvc;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Utilities;
using MaterialCodeSelectionPlatform.Web.Common;
using MaterialCodeSelectionPlatform.Web.Utilities;
using Microsoft.AspNetCore.Http;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class UserController : BaseController<IUserService, User>
    {

        public UserController(IUserService userService)
        {
            Service = userService;
        }

        public IActionResult Index()
        {
            //应用管理员，只能创建普通用户

            if (RoleId == 0)
            {
                ViewData["IsSuperAdmin"] = true;
            }
            else
            {
                ViewData["IsSuperAdmin"] = false;
            }

            return View();
        }

        public IActionResult Detail(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        public IActionResult AddOrEditPage(string id)
        {
            ViewData["id"] = id;
            if (RoleId == 0)
            {
                ViewData["IsSuperAdmin"] = true;
            }
            else
            {
                ViewData["IsSuperAdmin"] = false;
            }

            return View();
        }

        public IActionResult ChangePwd(string id)
        {
            ViewData["id"] = id;
            return View();
        }


        public IActionResult ChangeRole(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        public IActionResult SetProject(string id)
        {
            ViewData["id"] = id;
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetDataList(string name, int role, int status, int page, int limit)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;

            UserSearchCondition userSearchCondition = new UserSearchCondition();
            userSearchCondition.Page = dataPage;
            userSearchCondition.Name = name;
            userSearchCondition.Role = role;
            userSearchCondition.Status = status;
            var list = await Service.GetDataList(userSearchCondition);
            return ConvertListResult(list, dataPage);
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<IActionResult> SavePwd(string id, string pwd)
        {
            var model = await Service.GetAsync(id);
            model.Password = pwd;
            var result = await Service.UpdateAsync(model);
            return ConvertSuccessResult(result);
        }
        //权限先做到页面级别就好。
        //系统管理员 所有模块
        //应用管理员 除去用户管理
        //普通用户 只能物资选码
        /// <summary>
        /// 修改角色
        /// 角色的需求是
        /// 系统管理员 可以 指派应用管理员。
        /// 应用管理员不能把其他人设置成应用管理员。或者超级管理员。。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="role"></param>
        /// <returns></returns>

        public async Task<IActionResult> SaveRole(string id, int role)
        {
            var model = await Service.GetAsync(id);
            model.Role = role;
            var result = await Service.UpdateAsync(model);
            return ConvertSuccessResult(result);
        }


        /// <summary>
        /// 验证邮箱 和登录名是否重复
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IActionResult> VerifData(string value, string type, string id)
        {
            var Success = true;
            switch (type)
            {
                case "LoginName":
                    Success = await Service.VerifLoginNameAsync(value, id);
                    break;
                default:
                    break;
            }
            return ConvertSuccessResult(Success);
        }

        /// <summary>
        /// 获取分配资源待分配树的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetLeftDatas(string Id)
        {
            var list = await Service.GetLeftProjects(Id);
            List<TreeItemModel> result = new List<TreeItemModel>();
            foreach (var top in list)
            {
                TreeItemModel treeItemModel = new TreeItemModel();
                treeItemModel.title = top.Name;
                treeItemModel.value = top.Id;
                treeItemModel.disabled = false;
                treeItemModel.parentId = Guid.Empty.ToString();
                result.Add(treeItemModel);
            }
            return Json(result);
        }

        /// <summary>
        /// 获取分配资源已分配树的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetRightDatas(string Id)
        {
            var list = await Service.GetRightProjects(Id);
            List<TreeItemModel> result = new List<TreeItemModel>();
            foreach (var top in list)
            {
                TreeItemModel treeItemModel = new TreeItemModel();
                treeItemModel.title = top.Name;
                treeItemModel.value = top.Id;
                treeItemModel.disabled = false;
                treeItemModel.parentId = Guid.Empty.ToString();
                result.Add(treeItemModel);
            }
            return Json(result);
        }

        /// <summary>
        /// 保存已经分配的项目
        /// </summary>
        /// <param name="projectIds"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveUserProject(List<string> projectIds, string id)
        {
            var result = await Service.SaveUserProjects(projectIds, id, UserId);
            return ConvertSuccessResult(true);
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
                var result = importUserData(filePath, out errorPath, out errorMsg);
                if (result)
                {
                    return ConvertSuccessResult(null, "导入完成!");
                }
                else
                {
                    if (errorPath.IsNullOrEmpty())
                    {
                        return ConvertFailResultStr(null, errorMsg);
                    }
                    else
                    {
                        HttpContext.Session.SetString("ErrorUserPath", errorPath);
                        return ConvertFailResultStr(null, "haveError");
                    }
                }
            }
            else
            {
                return ConvertFailResultStr(null, "请选择文件");
            }
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool importUserData(string filePath, out string errorPath, out string errorMsg)
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
            mobColumns.Add("登录名");
            mobColumns.Add("姓名");
            mobColumns.Add("域账户");
            mobColumns.Add("性别");
            mobColumns.Add("角色");
            mobColumns.Add("密码");

            foreach (var mobColumn in mobColumns)
            {
                errorTable.Columns.Add(mobColumn, typeof(string));
                if (table.Columns.Contains(mobColumn) == false)
                {
                    errorMsg += mobColumn + ",";
                }
            }

            if (string.IsNullOrEmpty(errorMsg) == false)
            {
                errorMsg = "导入的文件缺失" + errorMsg.TrimEnd(',') + "列";
                return false;
            }

            errorTable.Columns.Add("错误信息", typeof(string));

            foreach (DataRow tableRow in table.Rows)
            {
                var errRowMsg = string.Empty;
                var loginName = tableRow["登录名"]?.ToString();
                var name = tableRow["姓名"]?.ToString();
                var domainName = tableRow["域账户"]?.ToString();
                var sex = tableRow["性别"]?.ToString();
                var role = tableRow["角色"]?.ToString();
                var password = tableRow["密码"]?.ToString();


                if (string.IsNullOrEmpty(loginName))
                {
                    errRowMsg += "登录名必填,";
                }

                if (string.IsNullOrEmpty(name))
                {
                    errRowMsg += "姓名必填,";
                }

                if (string.IsNullOrEmpty(password))
                {
                    errRowMsg += "密码必填,";
                }

                if (string.IsNullOrEmpty(role))
                {
                    errRowMsg += "角色必填,";
                }

                if (errRowMsg.IsNotNullAndNotEmpty())
                {
                    DataRow dr = errorTable.NewRow();
                    dr = insertToErroTabel(dr, errRowMsg, tableRow);
                    errorTable.Rows.Add(dr);
                    continue;
                }

                var roleIndex = -1;
                switch (role.Trim())
                {
                    case "超级管理员":
                        roleIndex = 0;
                        break;
                    case "应用管理员":
                        roleIndex = 1;
                        break;
                    case "普通用户":
                        roleIndex = 2;
                        break;
                }
                if (roleIndex == -1)
                {
                    errRowMsg += "角色名不正确,";
                }
                else
                {
                    //应用管理员只能导入普通用户
                    if (RoleId == 1)
                    {
                        if (roleIndex < 2 && roleIndex > -1)
                        {
                            errRowMsg += $"您不能导入角色是{role}的用户,";
                        }
                    }

                }
                var sexIndex = -1;
                if (sex.IsNotNullAndNotEmpty())
                {
                    switch (sex.Trim())
                    {
                        case "男":
                            sexIndex = 0;
                            break;
                        case "女":
                            sexIndex = 1;
                            break;
                        default:
                            errRowMsg += "性别不正确,";
                            break;
                    }
                }



                if (errRowMsg.IsNotNullAndNotEmpty())
                {
                    DataRow dr = errorTable.NewRow();
                    dr = insertToErroTabel(dr, errRowMsg, tableRow);
                    errorTable.Rows.Add(dr);
                    continue;
                }
                //根据装置编码，判断是否存在

                var User = Service.GetByParentId("LoginName", loginName).Result.FirstOrDefault();

                if (User == null)//新增
                {
                    User = new User();
                    User.Id = Guid.NewGuid().ToString();
                    User.Name = name;
                    if (sexIndex > -1)
                    {
                        User.Sex = sexIndex;
                    }

                    User.Role = roleIndex;
                    User.DomainUserName = domainName;
                    User.Password = CommonHelper.ToMD5(password);
                    User.LoginName = loginName.Trim();
                    User.CreateTime = DateTime.Now;
                    User.CreateUserId = UserId;
                    User.Flag = 0;
                    User.Status = 0;
                    User.LastModifyUserId = UserId;
                    User.LastModifyTime = DateTime.Now;
                    var d1 = Service.SaveAsync(User).Result;

                }
                else//修改
                {

                    User.Name = UserName;
                    if (sexIndex > -1)
                    {
                        User.Sex = sexIndex;
                    }

                    User.Role = roleIndex;
                    User.DomainUserName = domainName;
                    User.Password = CommonHelper.ToMD5(password);
                    User.LastModifyUserId = UserId;
                    User.LastModifyTime = DateTime.Now;
                    var d = Service.UpdateAsync(User).Result;

                }
            }

            if (errorTable.Rows.Count > 0)
            {
                var ext = Path.GetExtension(filePath);
                errorPath = filePath.Replace(ext, "_Error" + ext);
                ExcelHelper.WriteTableToExcel(new List<DataTable>() { errorTable }, new List<string>() { "用户导入" },
                    errorPath);
                return false;
            }
            else
            {
                return true;
            }

        }

        private DataRow insertToErroTabel(DataRow dr, string errorMsg, DataRow sourceRow)
        {
            dr["登录名"] = sourceRow["登录名"];
            dr["姓名"] = sourceRow["姓名"];
            dr["域账户"] = sourceRow["域账户"];
            dr["性别"] = sourceRow["性别"];
            dr["角色"] = sourceRow["角色"];
            dr["密码"] = sourceRow["密码"];
            dr["错误信息"] = errorMsg;
            return dr;
        }


        /// <summary>
        /// 下载错误文件
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadErrorFile()
        {
            var errorPath = HttpContext.Session.GetString("ErrorUserPath");
            return DownLoad(errorPath);
        }


        /// <summary>
        /// 下载错误文件
        /// </summary>
        /// <returns></returns>
        public IActionResult DownloadTemplateFile()
        {
            var path = System.IO.Path.Combine(appBasePath, "Templates", "User.xlsx");
            return DownLoad(path);
        }
    }
}