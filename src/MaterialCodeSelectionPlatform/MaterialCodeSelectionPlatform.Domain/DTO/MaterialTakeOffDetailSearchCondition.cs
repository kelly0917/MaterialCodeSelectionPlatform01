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
    }
}