namespace MaterialCodeSelectionPlatform.Domain
{
    public class MtoSearchCondition:SConditionBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MtoId { get; set; }
        /// <summary>
        /// 【0：工作中，待审批】
        /// </summary>
        public int Type { get; set; }
    }
}