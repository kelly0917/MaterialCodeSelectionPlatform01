using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class MaterialTakeOffDetailServiceImpl
    {
        /// <summary>
        /// 搜索 物料管理
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetailDto>> GetManagerList(
            MaterialTakeOffDetailSearchCondition searchCondition)
        {
            return await _MaterialTakeOffDetailDao.GetManagerList(searchCondition);
        }


        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="materialTakeOffDetails"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMaterialTakeOffDetail(List<MaterialTakeOffDetail> materialTakeOffDetails)
        {
            return await _MaterialTakeOffDetailDao.UpdateMaterialTakeOffDetail(materialTakeOffDetails);
        }
    }
}