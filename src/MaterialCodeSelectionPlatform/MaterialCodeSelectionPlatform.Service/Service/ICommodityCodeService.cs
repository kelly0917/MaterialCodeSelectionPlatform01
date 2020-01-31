using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial interface ICommodityCodeService
    {
        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        Task<List<CommodityCodeDto>> GetCommodityCodeDataList(CommodityCodeSerachCondition condition);
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id">物资编码Id</param>
        /// <returns></returns>
        Task<List<CommodityCodeAttribute>> GetAttributeList(string id);
        /// <summary>
        /// 选择【物资编码】的采购码
        /// </summary>
        /// <param name="commodityCodeId">物资编码Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task<List<PartNumberReport>> GetCommodityCodePartNumberList(string commodityCodeId, string userId);
    }
}