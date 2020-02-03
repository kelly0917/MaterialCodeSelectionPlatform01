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
        public List<string> ValueList { get; set; }
    }   

    /// <summary>
    /// 物资编码属性 DTO
    /// </summary>
    public class CommodityCodeAttributeDto : CommodityCodeAttribute
    {
       
    }
}