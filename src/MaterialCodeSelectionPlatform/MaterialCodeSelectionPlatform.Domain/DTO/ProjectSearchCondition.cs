namespace MaterialCodeSelectionPlatform.Domain
{
    public class ProjectSearchCondition:SConditionBase
    {
        public string UserId { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public int Status { get; set; }

    }
}