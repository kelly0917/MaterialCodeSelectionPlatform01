using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class MaterialTakeOffDetailCSVList
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// CSV
        /// </summary>
        public List<MaterialTakeOffDetailCSV> CSVList { get; set; } = new List<MaterialTakeOffDetailCSV>();
        /// <summary>
        /// 结果
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 日志跟踪
        /// </summary>
        public string LogMsg { get; set; }
        /// <summary>
        /// 错误日志（给用户看）
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LogPath { get; set; }

        public string FileName { get; set; }
    }
    /// <summary>
    /// CSV导入
    /// </summary>
    public class MaterialTakeOffDetailCSV
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 设备code
        /// </summary>
        public string DeviceId { get; set; }
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
        /// <summary>
        /// 错误日志（给用户看）
        /// </summary>
        public string ErrorMsg { get; set; }
        public bool Success { get; set; }
    }
}