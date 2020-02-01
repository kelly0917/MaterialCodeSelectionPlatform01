using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Service;
using Microsoft.AspNetCore.Mvc;
using MaterialCodeSelectionPlatform.ManagerWeb;
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
        public async  Task<IActionResult> GetDataList(string name, int role, int status, int page, int limit)
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
                    Success =await Service.VerifLoginNameAsync(value,id);
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
            var list =await Service.GetLeftProjects(Id);
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
        public async Task<IActionResult> SaveUserProject(List<string> projectIds,string id)
        {
            var result =await Service.SaveUserProjects(projectIds, id, UserId);
            return ConvertSuccessResult(true);
        }





    }
}