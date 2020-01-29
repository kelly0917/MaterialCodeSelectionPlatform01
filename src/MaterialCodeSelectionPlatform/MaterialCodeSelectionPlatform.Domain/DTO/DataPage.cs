using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MaterialCodeSelectionPlatform.Domain
{
    [DataContract]
    public class DataPage
    {
        /// <summary>
        /// 每页记录数,从1开始
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        private int _PageNo = 1;
        /// <summary>
        /// 当前页数
        /// </summary>
        [DataMember]
        public int PageNo
        {
            get
            { return _PageNo; }
            set
            {
                if (value < 1)
                {
                    _PageNo = 1;
                }
                else
                {
                    _PageNo = value;
                }
            }
        }

        int _RecordCount = -1;
        /// <summary>
        /// 总记录数(返回值)，默认为-1;
        /// </summary>
        /// <remarks>如果需要重新统计总记录数，请设置为-1</remarks>
        [DataMember]
        public int RecordCount
        {
            get
            {
                return _RecordCount;
            }
            set
            {
                if (_RecordCount != value)
                {
                    _RecordCount = value;
                }

                //　页码在合法范围
                if (_RecordCount > -1)
                {
                    if (PageNo > PageCount)
                    {
                        PageNo = PageCount;
                    }
                }
            }
        }

        /// <summary>
        /// 读取总页数(返回值),默认为0
        /// </summary>
        [DataMember]
        public int PageCount
        {
            get
            {
                if (RecordCount < 1)
                {
                    return 0;
                }
                else
                {
                    return (RecordCount / PageSize) + ((RecordCount % PageSize) == 0 ? 0 : 1);
                }
            }
            set { }
        }

        /// <summary>
        /// 读取开始位置,从0开始
        /// </summary>
        public int StartPosition
        {
            get
            {
                return (PageNo - 1) * PageSize;
            }
        }

        /// <summary>
        /// 读取结束位置
        /// </summary>
        public int EndPosition
        {
            get
            {
                return PageNo * PageSize - 1;
            }
        }
    }
}
