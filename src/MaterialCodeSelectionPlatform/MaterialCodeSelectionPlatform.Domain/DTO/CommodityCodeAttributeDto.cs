using MaterialCodeSelectionPlatform.Domain.Entities;
using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    /// 物资类型属性
    /// </summary>
    public class ComponentTypeAttribute
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 属性值列表（一个属性名称对应多个属性值）
        /// </summary>
        public List<AttriValue> AttributeValueList { get; set; }
    }
    /// <summary>
    /// 属性值
    /// </summary>
    public class AttriValue
    {
        /// <summary>
        /// 属性值ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 物资编码属性 DTO
    /// </summary>
    public class CommodityCodeAttributeDto : CommodityCodeAttribute
    {
       
    }
}