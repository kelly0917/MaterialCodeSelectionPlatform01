using System.Collections.Generic;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    ///保存【物资汇总明细表】条件
    /// </summary>
    public class PartNumberCondition
    {
        public List<PartNumberDto> PartNumberDtoList { get; set; }
        /// <summary>
        /// 物资编码Id
        /// </summary>
        public string CommodityCodeId { get; set; }
        /// <summary>
        /// 项目Id
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// 装置Id
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }

    }
}