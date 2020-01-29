using System.Collections.Generic;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Domain.Entities;

namespace CommodityCodeSelectionPlatform.Service
{
    public partial interface ICommodityCodeService
    {
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        Task<List<CommodityCodeAttribute>> GetAttributeById(string id, int languageType);
    }
}