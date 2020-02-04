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
        /// 项目名称 
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 用户名称 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 当前时间
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 装置编码
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 装置名称 
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 装置描述
        /// </summary>
        public string DeviceRemark { get; set; }

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
        /// 采购码中文长描述(表：MaterialTakeOffDetail)
        /// </summary>
        public string CN_PartNumberLongDesc { get; set; }
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