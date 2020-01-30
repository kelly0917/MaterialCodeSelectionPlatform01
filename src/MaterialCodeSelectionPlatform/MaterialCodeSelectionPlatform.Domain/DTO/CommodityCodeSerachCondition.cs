using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain.DTO
{
    /// <summary>
    ///物资编码选择条件
    /// </summary>
    public class CommodityCodeSerachCondition : SConditionBase
    {
        /// <summary>
        /// 物资编码Id
        /// </summary>
        public List<string> CommodityCodeId { get; set; }

    }
}