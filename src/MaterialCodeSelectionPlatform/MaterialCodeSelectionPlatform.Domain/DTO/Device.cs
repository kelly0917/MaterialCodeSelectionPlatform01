using SqlSugar;

namespace MaterialCodeSelectionPlatform.Domain.Entities
{
    public partial class Device
    {  
        /// <summary>
        /// 项目名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string ProjectCode { get; set; }
    }
}