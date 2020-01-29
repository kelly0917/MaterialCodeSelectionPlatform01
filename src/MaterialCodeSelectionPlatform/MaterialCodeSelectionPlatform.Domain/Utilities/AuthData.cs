using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialCodeSelectionPlatform.Domain.Utilities
{
    /// <summary>
    /// 用户拥有的权限数据信息
    /// </summary>
    public class AuthData
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称-模块/角色/用户组
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 模块操作权限值
        /// </summary>
        public long ActionValue { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SeqNo { get; set; }

        /// <summary>
        /// 机构类型id
        /// </summary>
        public string OrgTypeId { get; set; }
        ///// <summary>
        ///// 机构类型名称
        ///// </summary>
        //public string OrgTypeName { get; set; }
    }
}
