using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class ChangeHistoryDaoImpl
    {

        /// <summary>
        /// 查询变更历史
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<ChangeHistory>> GetDataList(ChangeHistorySearchCondition searchCondition)
        {
            var query = Db.Queryable<ChangeHistory>()
                .Where(c => c.MaterialTakeOffId == searchCondition.MaterialTakeOffId);
            var total = 0;
            var data = await query.OrderBy(c=>c.ChangeDate,OrderByType.Desc).ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);

            searchCondition.Page.RecordCount = data.Value;
            return data.Key;
        }

    }
}