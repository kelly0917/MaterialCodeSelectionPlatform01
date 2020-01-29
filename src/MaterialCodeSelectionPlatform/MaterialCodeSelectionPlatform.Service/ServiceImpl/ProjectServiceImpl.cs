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
    }
}