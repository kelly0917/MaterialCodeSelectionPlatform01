namespace MaterialCodeSelectionPlatform.Domain
{

    public class DicItemModel
    {
        public DicItemModel() { }
        /// <summary>
        /// 状态
        /// </summary>
        public string Name { get; set; }

        public string Code { get; set; }

        public string ParentId { get; set; }

        public int Status { get; set; }
    }
}