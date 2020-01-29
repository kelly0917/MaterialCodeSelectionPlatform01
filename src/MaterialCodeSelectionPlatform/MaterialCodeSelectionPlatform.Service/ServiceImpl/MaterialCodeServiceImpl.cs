using System.Collections.Generic;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Domain.Entities;

namespace CommodityCodeSelectionPlatform.Service
{
    public partial class CommodityCodeServiceImpl
    {
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public async Task<List<CommodityCodeAttribute>> GetAttributeById(string id, int languageType)
        {
            return await _CommodityCodeDao.GetAttributeById(id,languageType);
        }
    }
}