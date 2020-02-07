using System.Collections.Generic;
using MaterialCodeSelectionPlatform.Domain.DTO;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    ///物资编码选择条件
    /// </summary>
    public class CommodityCodeSerachCondition : SConditionBase
    {
        /// <summary>
        /// 物资类型ID
        /// </summary>
        public string ComponentTypeId { get; set; }
        ///// <summary>
        ///// 物资编码Id
        ///// </summary>
        //public List<string> CommodityCodeId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        ///// <summary>
        ///// 属性名称
        ///// </summary>
        //public string AttrName { get; set; }
        ///// <summary>
        ///// 属性值，多个值 用逗号隔开
        ///// </summary>
        //public List<string> AttrValue { get; set; }

        /// <summary>
        /// 选中的属性的物资编码Ids
        /// </summary>
        public List<AttributeModel> CompenetAttributes { get; set; }

    }
}