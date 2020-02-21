using MaterialCodeSelectionPlatform.Domain.Entities;
using System;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    /// 物资汇总表 DTO
    /// </summary>
    public class MaterialTakeOffDto : MaterialTakeOff
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 装置名称
        /// </summary>
        public string DeviceName { get; set; }
       /// <summary>
       /// 用户名
       /// </summary>
        public string UserName { get; set; }
        /// 时间
        /// </summary>
        public string TimeString
        {
            get
            {
                var ts =  DateTime.Now- LastModifyTime;               
                if (ts.Days > 7)
                {
                    return "1周前";
                }
                else if (ts.Days > 3)
                {
                    return "3天前";
                }
                else if (ts.Days >= 1)
                {
                    return "1天前";
                }
                else if (ts.Hours >= 3)
                {
                    return "3小时前";
                }
                else if (ts.Hours >= 1)
                {
                    return "1小时前";
                }
                else if (ts.TotalMinutes >= 30)
                {
                    return "30分钟前";
                }
                else if (ts.TotalMinutes >=3)
                {
                    return "3分钟前";
                }
                else
                {
                    return "刚刚";
                }
            }
        }
        /// <summary>
        /// 审批状态【工作中 】【已审批】 
        /// </summary>
        public string CheckStatusName
        {
            get
            {
                if (CheckStatus == 1)
                {
                    return "工作中";
                }
                else if (CheckStatus == 2)
                {
                    return "已审批";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}