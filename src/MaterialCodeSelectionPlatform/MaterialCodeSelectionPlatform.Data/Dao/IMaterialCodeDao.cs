using System.Collections.Generic;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Domain;
using CommodityCodeSelectionPlatform.Domain.Entities;

namespace CommodityCodeSelectionPlatform.Data
{
    public partial interface ICommodityCodeDao
    {
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        Task<List<CommodityCodeAttribute>> GetAttributeById(string id,int languageType);

        /// <summary>
        /// 获取物资编码下的采购编码以及属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        Task<List<PartNumberSizeAttribute>> GetAttributeSetList(string id, int languageType);


    }
}