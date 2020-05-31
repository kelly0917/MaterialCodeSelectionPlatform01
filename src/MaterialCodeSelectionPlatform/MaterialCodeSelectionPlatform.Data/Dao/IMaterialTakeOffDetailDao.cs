using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial interface IMaterialTakeOffDetailDao
    {
        /// <summary>
        /// 搜索 物料管理
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<List<MaterialTakeOffDetailDto>> GetManagerList(MaterialTakeOffDetailSearchCondition searchCondition);

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="materialTakeOffDetails"></param>
        /// <returns></returns>
        Task<bool> UpdateMaterialTakeOffDetail(List<MaterialTakeOffDetail> materialTakeOffDetails);

    }
}