using System.Collections.Generic;
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
        /// <param name="projectId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<List<ComponentTypeCount>> GetByCommodityCodeDesc(string projectId, string desc)
        {
            if (desc.IsNullOrEmpty())
            {
                return new List<ComponentTypeCount>();
            }

            var catalogQuery = Db.Queryable<ProjectCatalogMap>();
            if (projectId.IsNotNullAndNotEmpty())
            {
                catalogQuery = catalogQuery.Where(c => c.ProjectId == projectId);
            }
            //项目关联的编码库
            var catalogIds = await catalogQuery.Select(c => c.CatalogId).ToListAsync();

            var query = await Db.Queryable<ComponentType, CommodityCode>((t, c) => new object[]
                {
                    JoinType.Inner, t.Id == c.ComponentTypeId
                }).Where((t, c) => SqlFunc.Contains(c.CN_LongDesc, desc)).GroupBy((t, c) => t.Id).GroupBy((t, c) => t.Code).GroupBy((t, c) => t.Desc)

                .Select((t, c) => new ComponentTypeCount { Count = SqlFunc.AggregateCount(t.Id), ComponentTypeId = t.Id, ComponentTypeCode = t.Code, ComponentTypeDesc = t.Desc }).ToListAsync();

            return query;
        }
    }
}