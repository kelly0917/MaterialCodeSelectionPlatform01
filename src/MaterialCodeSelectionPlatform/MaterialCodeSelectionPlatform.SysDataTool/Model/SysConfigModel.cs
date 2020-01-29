namespace MaterialCodeSelectionPlatform.SysDataTool.Model
{
    /// <summary>
    /// 同步配置
    /// </summary>
    public class SysConfigModel
    {
        public string Id { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual string ConnectionString { get; set; }

        public virtual string CN_COMM_DESC_SHORT { get; set; }

        public virtual string EN_COMM_DESC_SHORT { get; set; }

        public virtual string RU_COMM_DESC_SHORT { get; set; }

        public virtual string CN_COMM_DESC_LONG { get; set; }

        public virtual string EN_COMM_DESC_LONG { get; set; }

        public virtual string RU_COMM_DESC_LONG { get; set; }

        public virtual string CN_PART_DESC_SHORT { get; set; }

        public virtual string EN_PART_DESC_SHORT { get; set; }

        public virtual string RU_PART_DESC_SHORT { get; set; }

        public virtual string CN_PART_DESC_LONG { get; set; }

        public virtual string EN_PART_DESC_LONG { get; set; }

        public virtual string RU_PART_DESC_LONG { get; set; }

        public virtual string CN_SIZE_DESC { get; set; }

        public virtual string EN_SIZE_DESC { get; set; }

        public virtual string RU_SIZE_DESC { get; set; }

        public virtual string COMM_REPRESENT_TYPE { get; set; }
    }
}