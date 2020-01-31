using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial interface IProjectService
    {
        /// <summary>
        /// 搜索项目
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<List<Project>> GetDataList(ProjectSearchCondition searchCondition);


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        Task<bool> VerifNameOrCodeAsync(string nameCode, string id);


        /// <summary>
        /// 获取待分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Catalog>> GetLeftCatalogs(string id);


        /// <summary>
        /// 获取已分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<Catalog>> GetRightCatalogs(string id);


        /// <summary>
        /// 保存用户分配的编码库
        /// </summary>
        /// <param name="catalogs"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> SaveProjectCatlogs(List<string> catalogs, string userId, string projectId);
    }
}