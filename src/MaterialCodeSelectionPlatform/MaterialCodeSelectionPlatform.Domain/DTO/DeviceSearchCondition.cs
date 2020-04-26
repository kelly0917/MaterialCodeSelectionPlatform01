namespace MaterialCodeSelectionPlatform.Domain
{
    public class DeviceSearchCondition:SConditionBase
    {
        
        public string ProjectId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

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