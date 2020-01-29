using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommodityCodeSelectionPlatform.Web
{
    /// <summary>
    /// layuiXtree 数据实体类
    /// </summary>
    public class TreeItemModel
    {
        public TreeItemModel()
        {
            data = new List<TreeItemModel>();
        }

        /// <summary>
        /// 节点标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 节点值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool disabled { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool @checked { get; set; }

        /// <summary>
        /// 父级Id
        /// </summary>
        public string parentId { get; set; }

        /// <summary>
        /// 操作值
        /// </summary>
        public long operationValue { get; set; }

        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool isNotEnd { get; set; }

        /// <summary>
        /// 授权方式
        /// 0增加授权 1减去授权
        /// </summary>
        public  int ? GrantType { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeItemModel> data { get; set; }
    }
}
