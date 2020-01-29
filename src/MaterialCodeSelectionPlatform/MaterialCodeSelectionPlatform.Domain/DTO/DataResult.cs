using System;
using System.Collections.Generic;
using System.Text;
using MaterialCodeSelectionPlatform.Domain;

namespace MaterialCodeSelectionPlatform.Domain
{
    /// <summary>
    /// 前后端统一返回数据类
    /// </summary>
   public class DataResult
    {
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 影响的记录数，一般针对返回列表使用
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 对象，可以是任何对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 文件的路径，主要针对上传下载
        /// </summary>
        public string Url { get; set; }
    }
    /// <summary>
    ///  前后端统一返回数据类
    /// </summary>    
    public class DataResult<T>
    {
        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 影响的记录数，一般针对返回列表使用
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 对象，可以是任何对象
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 类的对象
        /// </summary>
        public T Model { get; set; }
        /// <summary>
        /// 返回列表，通常用在分页数据，或列表数据
        /// </summary>
        public List<T> DataList { get; set; }
        /// <summary>
        /// 文件的路径，主要针对上传下载
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 分页导航：【PageSize：每页显示的记录数】 【PageNo：第几页】 【RecordCount：总记录数】 【PageCount：总页数】
        /// </summary>
        public DataPage Page { get; set; }

        public static DataResult<T> GetWrappedResultFromData(IList<T> entityList,DataPage page)
        {
            DataResult<T> result = new DataResult<T>();

            result.Success = true;
            result.Count = page.RecordCount;
            result.Data = entityList;
            result.Page = page;

            return result;
        }
    }
}
