namespace MaterialCodeSelectionPlatform.Domain.DTO
{

    public class MaterialTakeOffDetailSearchCondition:SConditionBase
    {
        public string ProjectId { get; set; }

        public string DeviceId { get; set; }

        public string ComponentTypeCode { get; set; }

        public string ComponentTypeDesc { get; set; }

        public string PartNumberCNDesc { get; set; }

        public string PartNumberENDesc { get; set; }

        public string PartNumberRUDesc { get; set; }

        public string CommodityCode { get; set; }

        public string PartNumberCode { get; set; }

        /// <summary>
        /// 排序列明
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 排序类型，0升序，1降序
        /// </summary>
        public int OrderByType { get; set; }
    }
}