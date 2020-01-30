using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    /// 物资编码DTO
    /// </summary>
    public class CommodityCodeDto: CommodityCode
    {
        /// <summary>
        /// 物资类型的名称 
        /// </summary>
        public string Desc { get; set; }
    }
}