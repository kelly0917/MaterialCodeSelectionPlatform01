using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class CommodityCodeDaoImpl
    {
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public async Task<List<CommodityCodeAttribute>> GetAttributeById(string id, int languageType)
        {
            //var query = Db.Queryable<CommodityCodeAttribute>((m, v) => new object[]
            //{
            //    JoinType.Left, m.Id == v.CommodityCodeAttributeId
            //}).Where((m, v) => m.CommodityCodeId == id && v.Type == languageType).Select((m,v)=>new CommodityCodeAttribute()
            //{
            //    Id=m.Id,
            //    AttributeName =  m.AttributeName,
            //    Value = v.Value
            //});

            //return await query.ToListAsync();

            return null;
        }

        /// <summary>
        /// 获取物资编码下的采购编码以及属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public async Task<List<PartNumberSizeAttribute>> GetAttributeSetList(string id, int languageType)
        {
            //var query = Db
            //    .Queryable<PartNumber, PartNumberSizeAttribute, PartNumberSizeDescription
            //    >((a, b, c) => new object[]
            //    {
            //        JoinType.Inner, a.Id == b.PartNumberId,
            //        JoinType.Inner, a.Id == b.PartNumberId
            //    });


            return null;

        }
        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public async Task<List<CommodityCodeDto>> GetCommodityCodeDataList(CommodityCodeSerachCondition condition)
        {
            #region sql
            /*
            SELECT b.[Desc], a.Id,a.Code,a.CN_ShortDesc,a.* FROM CommodityCode a
            LEFT JOIN ComponentType b ON a.ComponentTypeId=b.Id AND b.Status= 0
            WHERE a.Code LIKE '%%' 
            AND a.Id IN('62F59486-6CF8-42D9-8622-37C3090CEE0A')
            */
            #endregion

            var query = Db.Queryable<CommodityCode, ComponentType>((a,b)=>new object[] { JoinType.Left,a.ComponentTypeId==b.Id});
            query = query.Where((a, b) => a.Status==0);
            if (condition.Code.IsNotNullAndNotEmpty())
            {
                query = query.Where((a,b)=>a.Code.Contains(condition.Code));
            }
            if (condition.CommodityCodeId != null && condition.CommodityCodeId.Count > 0)
            {
                query = query.Where((a, b) => condition.CommodityCodeId.Contains(a.Id));
            }
            //query = query.Select((a, b) => (new CommodityCodeDto { Id =a.Id}));
                var total = 0;
                var data = await query.Select((a, b) => (new CommodityCodeDto { Id = a.Id,Desc=b.Desc,Code=a.Code, CN_ShortDesc = a.CN_ShortDesc })).ToPageListAsync(condition.Page.PageNo, condition.Page.PageSize, total);
                condition.Page.RecordCount = data.Value;
                return data.Key;              
        }
    }
}