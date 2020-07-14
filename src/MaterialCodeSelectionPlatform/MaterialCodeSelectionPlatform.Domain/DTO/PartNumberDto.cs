using MaterialCodeSelectionPlatform.Domain.Entities;
using SqlSugar;
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
        /// 版次
        /// </summary>
        public int? Version { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Revision { get; set; }
        /// <summary>
        /// 装置名称 
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 装置描述
        /// </summary>
        public string DeviceRemark { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string Approver { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string ApproveContent { get; set; }

        /// <summary>
        /// 物资编码类型名称
        /// </summary>
        public string ComponentTypeName { get; set; }
        /// <summary>
        /// CommodityCode 的中文描述
        /// </summary>
        public string C_CN_ShortDesc { get; set; }
        /// <summary>
        ///CommodityType 单位
        /// </summary>
        public string T_Unit { get; set; }
        /// <summary>
        /// 采购码
        /// </summary>
        public List<PartNumberReportDetail> PartNumberReportDetailList { get; set; }
    }
    public class PartNumberReportDetail : MaterialTakeOffDetail
    {

        #region PartNumber
        public int P_Seq { get; set; }
        public string P_Code { get; set; }
        public string P_CN_ShortDesc { get; set; }
        public string P_EN_ShortDesc { get; set; }
        public string P_RU_ShortDesc { get; set; }
        public string P_CN_LongDesc { get; set; }
        public string P_EN_LongDesc { get; set; }
        public string P_RU_LongDesc { get; set; }
        public string P_CN_SizeDesc { get; set; }
        public string P_EN_SizeDesc { get; set; }
        public string P_RU_SizeDesc { get; set; }
        #endregion
        #region CommodityCode
        public int C_Seq { get; set; }
        public string C_Code { get; set; }
        public string C_CN_ShortDesc { get; set; }
        public string C_EN_ShortDesc { get; set; }
        public string C_RU_ShortDesc { get; set; }
        public string C_CN_LongDesc { get; set; }
        public string C_EN_LongDesc { get; set; }
        public string C_RU_LongDesc { get; set; }
        #endregion
        #region CommodityCode
        public int T_Seq { get; set; }
        public string T_Code { get; set; }
        public string T_Desc { get; set; }
        public string T_Unit { get; set; }
        public string T_Discipline { get; set; }
        #endregion       
        /// <summary>
        /// 物资编码类型名称
        /// </summary>
        public string ComponentTypeName { get; set; }
        /// <summary>
        /// 裕量
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public double? AllowanceQty { get; set; }
       
        /// <summary>
        /// 总量
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal? AllQty
        {
            get
            {
                var allqty = DesignQty + (AllowanceQty != null ? AllowanceQty : 0);
                return decimal.Round(decimal.Parse(allqty.ToString()), 2);

            }
        }
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