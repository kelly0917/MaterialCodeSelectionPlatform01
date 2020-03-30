using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class ComponentTypeDaoImpl
    {
        /// <summary>
        /// 根据物资编码描述，获取对应的类型列表
        /// </summary>
        /// <param name="catalogId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<List<ComponentTypeCount>> GetByCommodityCodeDesc(string catalogId, string desc)
        {
            if (desc.IsNullOrEmpty())
            {
                return new List<ComponentTypeCount>();
            }
            var query = Db.Queryable<ComponentType, CommodityCode>((t, c) => new object[]
            {
                JoinType.Inner, t.Id == c.ComponentTypeId
            }).Where((t, c) => t.CatalogId == catalogId);

            var descList = desc.Split(@" ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in descList)
            {
                query = query.Where((t, c) => SqlFunc.Contains(c.CN_LongDesc, s));
            }

            var list =await query
                .GroupBy((t, c) => t.Id)
                .GroupBy((t, c) => t.Code)
                .GroupBy((t, c) => t.Desc)

                .Select((t, c) => new ComponentTypeCount { Count = SqlFunc.AggregateCount(t.Id), ComponentTypeId = t.Id, ComponentTypeCode = t.Code, ComponentTypeDesc = t.Desc }).ToListAsync();

            return list;
        }


        /// <summary>
        /// 获取物资类型的物资编码获取对应的属性和属性值
        /// </summary>
        /// <param name="compenentTypeId"></param>
        /// <returns></returns>
        public async Task<List<ComAttrModel>> GetAttributeByCompenetType(string compenentTypeId, string userInputText)
        {


            var query =  Db.Queryable<CommodityCodeAttribute, CommodityCode>((c, a) => new object[]
                {
                    JoinType.Inner, c.CommodityCodeId == a.Id
                }).Where((c, a) => c.ComponentTypeId == compenentTypeId).Where((c, a) => c.AttributeValue != null)
                .Where((c, a) => c.AttributeValue != "");


                  if (string.IsNullOrEmpty(userInputText) == false)
                  {
                      userInputText = userInputText.Trim();
                      query = query.Where((c, a) => a.CN_LongDesc.Contains(userInputText));
                  }

            var list =await query.ToListAsync();

            List<ComAttrModel> comAttrModels = new List<ComAttrModel>();
            var attributes = list.Select(c => c.AttributeName).Distinct().ToList();
            foreach (var attribute in attributes)
            {
                ComAttrModel cm = new ComAttrModel();
                cm.AttrbuteName = attribute;

                cm.AttributeValues = 
                    list.Where(c => c.AttributeName == attribute).Select(c => c.AttributeValue).Distinct().ToList();

                comAttrModels.Add(cm);
            }


            return comAttrModels;

        }
    }
}