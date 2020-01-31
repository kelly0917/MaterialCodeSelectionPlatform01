using MaterialCodeSelectionPlatform.Domain.Entities;
using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    /// 料表DTO
    /// </summary>
    public class PartNumberReport
    {
        /// <summary>
        /// 物资编码类型名称
        /// </summary>
        public string ComponentTypeName { get; set; }
        /// <summary>
        /// 采购码
        /// </summary>
        public List<PartNumberDto> PartNumberList { get; set; }
    }
    /// <summary>
    /// 采购码DTO
    /// </summary>
    public class PartNumberDto : PartNumber
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 装置Id
        /// </summary>
        public string DeviceId { get; set; }
       
        /// <summary>
        /// 物资编码类型名称
        /// </summary>
        public string ComponentTypeName { get; set; }
        /// <summary>
        /// 采购码数量
        /// </summary>
        public double DesignQty { get; set; }
    }
}