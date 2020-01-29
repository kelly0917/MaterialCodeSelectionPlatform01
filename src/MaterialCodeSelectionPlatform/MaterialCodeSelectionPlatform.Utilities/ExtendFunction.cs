namespace CommodityCodeSelectionPlatform.Utilities
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

    }
}