using MaterialCodeSelectionPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;


namespace MaterialCodeSelectionPlatform.Data
{
    public partial interface ICommodityCodeDao
    {
        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        Task<List<CommodityCodeDto>> GetCommodityCodeDataList(CommodityCodeSerachCondition condition);
    }
}
