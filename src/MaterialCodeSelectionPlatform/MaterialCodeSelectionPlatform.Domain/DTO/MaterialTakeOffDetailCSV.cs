using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain.DTO
{
    /// <summary>
    /// CSV导入
    /// </summary>
    public class MaterialTakeOffDetailCSV
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 设备code
        /// </summary>
        public string DeviceCode { get; set; }
        
        /// <summary>
        /// PartNumberCode对应的
        /// </summary>
        public Dictionary<string, double> PartNumberDesignQty { get; set; } = new Dictionary<string, double>();

        /// <summary>
        /// 日志跟踪
        /// </summary>
        public string LogMsg { get; set; }
    }
}