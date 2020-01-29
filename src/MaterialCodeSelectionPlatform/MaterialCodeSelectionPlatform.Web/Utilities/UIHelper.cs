using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using CommodityCodeSelectionPlatform.Domain;
using Microsoft.Extensions.DependencyInjection;


namespace CommodityCodeSelectionPlatform.Web.Common
{
    public static class UIHelper
    {
        private static ILog log = LogHelper.GetLogger<Object>();
        public static string AppBasePath = AppDomain.CurrentDomain.BaseDirectory;

     

        public static void AddSingleHelper(this IServiceCollection services)
        {
         
            
        }

        public static void LogError(this ILog log,Exception e)
        {
            log.Error("", e);
        }


        /// <summary>
        /// 转换url参数到查询实体类
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="entity"></param>
        /// <param name="urlParamDtos"></param>
        /// <returns></returns>
        public static M ConvertUrlParentToEntity<M>(M entity, List<UrlParamDTO> urlParamDtos) where M : new()
        {
            if (entity == null)
            {
                entity = new M();
            }

            var propertys = entity.GetType().GetProperties();
            foreach (var propertyInfo in propertys)
            {
                var pName = propertyInfo.Name;
                var urlModel = urlParamDtos.Where(c => c.Key.Equals(pName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (urlModel != null && urlModel.Value.Count > 0)
                {
                    var propertyTypeStr = propertyInfo.PropertyType.ToString();
                    try
                    {
                        if (propertyTypeStr.Contains("DateTime"))
                        {
                            entity.GetType().GetProperty(pName).SetValue(entity, DateTime.Parse(urlModel.Value[0]));
                        }
                        else if (propertyTypeStr.Contains("Double"))
                        {
                            entity.GetType().GetProperty(pName).SetValue(entity, double.Parse(urlModel.Value[0]));
                        }
                        else if (propertyTypeStr.Contains("Float"))
                        {
                            entity.GetType().GetProperty(pName).SetValue(entity, float.Parse(urlModel.Value[0]));
                        }
                        else if (propertyTypeStr.Contains("Int"))
                        {
                            entity.GetType().GetProperty(pName).SetValue(entity, int.Parse(urlModel.Value[0]));
                        }
                        else if (propertyTypeStr.Contains("String"))
                        {
                            entity.GetType().GetProperty(pName).SetValue(entity, urlModel.Value[0]);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error($"属性：{pName}，类型是{propertyTypeStr},不匹配值：{urlModel.Value[0]}");
                    }
                }
            }

            return entity;
        }



    }
}