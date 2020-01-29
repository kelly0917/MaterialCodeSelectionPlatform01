using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.Models
{
    public class MenuModel
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        public string FeatureId { get; set; }

        /// <summary>
        /// 子模块集合
        /// </summary>
        public List<MenuModel> ChildrenMenus { get; set; }



    }
}
