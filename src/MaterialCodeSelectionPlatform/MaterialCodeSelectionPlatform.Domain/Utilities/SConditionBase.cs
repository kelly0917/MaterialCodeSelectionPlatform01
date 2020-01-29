using System;
using System.Collections.Generic;
using System.Text;

namespace CommodityCodeSelectionPlatform.Domain
{
    /// <summary>
    /// 搜索条件基类
    /// </summary>
    public abstract class SConditionBase
    {

        /// <summary>
        /// 分页条件
        /// </summary>
        public DataPage Page { get; set; }

    }
}
