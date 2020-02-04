using System;
using System.Collections.Generic;
using System.Linq;

namespace MaterialCodeSelectionPlatform.Utilities
{
    public static class ExtendFunction
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullAndNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string SerializeToString(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 判断两个集合是否是相等的(所有的元素及数量都相等)
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="sourceCollection">源集合列表</param>
        /// <param name="targetCollection">目标集合列表</param>
        /// <returns>两个集合相等则返回True,否则返回False</returns>
        public static bool EqualList<T>(this IList<T> sourceCollection, IList<T> targetCollection) where T : IEquatable<T>
        {
            if (sourceCollection == null || targetCollection == null)
            {
                return true;
            }
            else if (sourceCollection?.Count != targetCollection?.Count)
            {
                return false;
            }
            else if (sourceCollection.All(targetCollection.Contains) && sourceCollection.Count == targetCollection.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}