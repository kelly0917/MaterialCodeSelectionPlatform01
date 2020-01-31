using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class ProjectServiceImpl
    {
        /// <summary>
        /// 搜索项目
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetDataList(ProjectSearchCondition searchCondition)
        {
            return await _ProjectDao.GetDataList(searchCondition);
        }


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public async Task<bool> VerifNameOrCodeAsync(string nameCode, string id)
        {
            return await _ProjectDao.VerifNameOrCodeAsync(nameCode,id);
        }


        /// <summary>
        /// 获取待分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Catalog>> GetLeftCatalogs(string id)
        {
            return await _ProjectDao.GetLeftCatalogs(id);
        }


        /// <summary>
        /// 获取已分配的项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Catalog>> GetRightCatalogs(string id)
        {
            return await _ProjectDao.GetRightCatalogs(id);
        }

        /// <summary>
        /// 保存用户分配的编码库
        /// </summary>
        /// <param name="catalogs"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> SaveProjectCatlogs(List<string> catalogs, string userId, string projectId)
        {
            return await _ProjectDao.SaveProjectCatlogs(catalogs, userId, projectId);
        }



        /// <summary>
        /// 获取待分配的用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<User>> GetLeftUsers(string id)
        {
            return await _ProjectDao.GetLeftUsers(id);
        }


        /// <summary>
        /// 获取已分配的编码库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<User>> GetRightUsers(string id)
        {
            return await _ProjectDao.GetRightUsers(id);
        }


        /// <summary>
        /// 保存用户分配的编码库
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> SaveUserProjects(List<string> userIds, string userId, string projectId)
        {
            return await _ProjectDao.SaveUserProjects(userIds,userId,projectId);
        }
    }
}