using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class ProjectDaoImpl
    {


        /// <summary>
        /// 搜索项目
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<Project>> GetDataList(ProjectSearchCondition searchCondition)
        {
            var query = Db.Queryable<Project>();

            if (searchCondition.Name.IsNotNullAndNotEmpty())
            {
                query = query.Where(c => c.Name.Contains(searchCondition.Name));
            }

            if (searchCondition.Code.IsNotNullAndNotEmpty())
            {
                query = query.Where(c => c.Code.Contains(searchCondition.Code));
            }

            if (searchCondition.Status != -1)
            {
                query = query.Where(c => c.Status == searchCondition.Status);
            }

            var total = 0;
            var data = await query.ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);
            searchCondition.Page.RecordCount = data.Value;
            return data.Key;
        }


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public async Task<bool> VerifNameOrCodeAsync(string nameCode, string id)
        {
            var result = false;
            if (string.IsNullOrEmpty(id))
            {
                var model =await DbContext.AsQueryable().Where(c => c.Name == nameCode || c.Code == nameCode).FirstAsync();
                if (model == null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                var model = await DbContext.AsQueryable().Where(c => c.Name == nameCode || c.Code == nameCode).Where(c=>c.Id !=id).FirstAsync();
                if (model == null)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return await Task.FromResult(result);
        }
    }
}