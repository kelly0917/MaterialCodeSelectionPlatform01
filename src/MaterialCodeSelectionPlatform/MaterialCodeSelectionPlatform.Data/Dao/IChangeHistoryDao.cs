using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial interface IChangeHistoryDao
    {
        /// <summary>
        /// 查询变更历史
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<List<ChangeHistory>> GetDataList(ChangeHistorySearchCondition searchCondition);



    }
}