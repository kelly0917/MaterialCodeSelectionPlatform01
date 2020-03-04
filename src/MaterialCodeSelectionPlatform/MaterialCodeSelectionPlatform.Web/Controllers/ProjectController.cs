using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.ManagerWeb;
using MaterialCodeSelectionPlatform.Service;
using MaterialCodeSelectionPlatform.Web.Common;
using MaterialCodeSelectionPlatform.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace MaterialCodeSelectionPlatform.Web.Controllers
{
    public class ProjectController : BaseController<IProjectService, Project>
    {
        private ILog log = LogHelper.GetLogger<ProjectController>();
        private ICatalogService catalogService;
        public ProjectController(IProjectService projectService,ICatalogService catalogService)
        {
            Service = projectService;
            this.catalogService = catalogService;
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

        public IActionResult SetCataLogPage(string id)
        {
            ViewData["id"] = id;
            return View();
        }

        public IActionResult SetUserPage(string id)
        {
            ViewData["id"] = id;
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
        public async Task<IActionResult> GetDataList(string name, string code, int status, int page, int limit)
        {
            DataPage dataPage = new DataPage();
            dataPage.PageNo = page;
            dataPage.PageSize = limit;

            ProjectSearchCondition projectSearchCondition = new ProjectSearchCondition();
            projectSearchCondition.Page = dataPage;
            projectSearchCondition.Code = code;
            projectSearchCondition.Name = name;
            projectSearchCondition.Status = status;
            var list = await Service.GetDataList(projectSearchCondition);
            return ConvertListResult(list, dataPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IActionResult> VerifData(string value, string type, string id)
        {
            var Success = true;
            switch (type)
            {
                case "name":
                    Success = await Service.VerifNameOrCodeAsync(value, id);
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
            var list = await Service.GetLeftCatalogs(Id);
            List<TreeItemModel> result = new List<TreeItemModel>();
            foreach (var top in list)
            {
                TreeItemModel treeItemModel = new TreeItemModel();
                treeItemModel.title = top.Description;
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
            var list = await Service.GetRightCatalogs(Id);
            List<TreeItemModel> result = new List<TreeItemModel>();
            foreach (var top in list)
            {
                TreeItemModel treeItemModel = new TreeItemModel();
                treeItemModel.title = top.Description;
                treeItemModel.value = top.Id;
                treeItemModel.disabled = false;
                treeItemModel.parentId = Guid.Empty.ToString();
                result.Add(treeItemModel);
            }
            return Json(result);
        }

        /// <summary>
        /// 保存已经分配的编码库
        /// </summary>
        /// <param name="catalogs"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveProjectCatlog(List<string> catalogs, string id)
        {
            var result =await Service.SaveProjectCatlogs(catalogs, UserId, id);
            return ConvertSuccessResult(true);
        }




        /// <summary>
        /// 获取分配资源待分配树的数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetLeftUserDatas(string Id)
        {
            var list = await Service.GetLeftUsers(Id);
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
        public async Task<IActionResult> GetRightUserDatas(string Id)
        {
            var list = await Service.GetRightUsers(Id);
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
        /// 保存已经分配的用户
        /// </summary>
        /// <param name="userids"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveUserProject(List<string> userids, string id)
        {
            var result = await Service.SaveUserProjects(userids, UserId, id);
            return ConvertSuccessResult(true);
        }


     
    }
}