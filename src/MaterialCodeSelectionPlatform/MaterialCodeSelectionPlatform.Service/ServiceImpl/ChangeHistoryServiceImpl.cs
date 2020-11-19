using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class ChangeHistoryServiceImpl
    {
        /// <summary>
        /// 查询变更历史
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<ChangeHistory>> GetDataList(ChangeHistorySearchCondition searchCondition)
        {
            return await _ChangeHistoryDao.GetDataList(searchCondition);
        }
    }
}