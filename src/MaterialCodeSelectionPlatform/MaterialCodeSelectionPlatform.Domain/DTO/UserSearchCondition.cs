namespace MaterialCodeSelectionPlatform.Domain
{
    public class UserSearchCondition:SConditionBase
    {
        /// <summary>
        /// 姓名或登录名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public int Role { get; set; }


        public int Status { get; set; }
        
    }
}