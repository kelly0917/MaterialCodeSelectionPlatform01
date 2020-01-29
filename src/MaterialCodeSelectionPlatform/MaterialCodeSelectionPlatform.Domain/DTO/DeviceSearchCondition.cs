namespace CommodityCodeSelectionPlatform.Domain
{
    public class DeviceSearchCondition:SConditionBase
    {
        
        public string ProjectId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}