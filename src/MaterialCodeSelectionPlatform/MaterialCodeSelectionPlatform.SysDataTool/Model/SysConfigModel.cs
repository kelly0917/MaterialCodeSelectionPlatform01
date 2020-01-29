namespace CommodityCodeSelectionPlatform.SysDataTool.Model
{
    /// <summary>
    /// 同步配置
    /// </summary>
    public class SysConfigModel
    {
        public string ConnectionString { get; set; }
        public string CAT_ID { get; set; }


        public string CAT_NAME { get; set; }
        public string CN_COMM_DESC_SHORT { get; set; }
        public string CN_COMM_DESC_LONG { get; set; }
        public string EN_COMM_DESC_SHORT { get; set; }
        public string EN_COMM_DESC_LONG { get; set; }
        public string RU_COMM_DESC_SHORT { get; set; }
        public string RU_COMM_DESC_LONG { get; set; }

        public string CN_PART_DESC_SHORT { get; set; }
        public string CN_PART_DESC_LONG { get; set; }
        public string EN_PART_DESC_SHORT { get; set; }
        public string EN_PART_DESC_LONG { get; set; }
        public string RU_PART_DESC_SHORT { get; set; }
        public string RU_PART_DESC_LONG { get; set; }

        public string CN_SIZE_DESC { get; set; }
        public string EN_SIZE_DESC { get; set; }
        public string RU_SIZE_DESC { get; set; }

        public string COMM_REPRESENT_TYPE { get; set; }
    }
}