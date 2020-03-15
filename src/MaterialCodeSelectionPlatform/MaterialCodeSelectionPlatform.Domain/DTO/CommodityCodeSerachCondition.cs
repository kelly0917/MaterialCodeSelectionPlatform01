using System.Collections.Generic;
using System.IO;
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
        public string InputText { get; set; }
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

        /// <summary>
        /// 排序列明
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 排序类型，0升序，1降序
        /// </summary>
        public int OrderByType { get; set; }

        public string CatelogId { get; set; }


    }
}