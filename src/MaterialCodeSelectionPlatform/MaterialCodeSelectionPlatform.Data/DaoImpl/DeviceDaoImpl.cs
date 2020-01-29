using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class DeviceDaoImpl
    {
        /// <summary>
        /// 搜索装置
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<Device>> GetDataList(DeviceSearchCondition searchCondition)
        {
            var query = Db.Queryable<Device,Project>((d,p)=> new object[]
            {
                JoinType.Left,d.ProjectId == p.Id
            });

            if (searchCondition.Name.IsNotNullAndNotEmpty())
            {
                query = query.Where((d, p) => d.Name.Contains(searchCondition.Name));
            }

            if (searchCondition.Code.IsNotNullAndNotEmpty())
            {
                query = query.Where((d, p) => d.Code.Contains(searchCondition.Code));
            }

            if (searchCondition.ProjectId != EmptyGuid)
            {
                query = query.Where((d, p) => d.ProjectId == searchCondition.ProjectId);
            }

            var total = 0;
            var data = await query.Select((d, p) =>new Device()
            {
                Id =  d.Id,
                ProjectId = d.ProjectId,
                ProjectName = p.Name +"("+p.Code+")",
                Name = d.Name,
                Code = d.Code,
                Remark = d.Remark,
                Status = d.Status,
                Flag = d.Flag,
                CreateUserId = d.CreateUserId,
                CreateTime = d.CreateTime,
                LastModifyTime = d.LastModifyTime,
                LastModifyUserId = d.LastModifyUserId
            }).ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);

            searchCondition.Page.RecordCount = data.Value;
            return data.Key;
        }


        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public async Task<bool> VerifNameOrCodeAsync(string nameCode, string projectId, string id)
        {
            var result = false;
            if (string.IsNullOrEmpty(id))
            {
                var model = await DbContext.AsQueryable().Where(c => c.Name == nameCode || c.Code == nameCode).Where(c=>c.ProjectId == projectId).FirstAsync();
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
                var model = await DbContext.AsQueryable().Where(c => c.Name == nameCode || c.Code == nameCode).Where(c => c.Id != id).Where(c => c.ProjectId == projectId).FirstAsync();
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