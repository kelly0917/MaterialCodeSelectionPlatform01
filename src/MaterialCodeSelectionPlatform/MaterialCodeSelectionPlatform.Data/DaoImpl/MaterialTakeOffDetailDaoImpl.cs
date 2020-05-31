using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using SqlSugar;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class MaterialTakeOffDetailDaoImpl
    {
        /// <summary>
        /// 搜索 物料管理
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetailDto>> GetManagerList(MaterialTakeOffDetailSearchCondition searchCondition)
        {
            var query = Db.Queryable<CommodityCode, ComponentType, PartNumber, MaterialTakeOffDetail>((a, b, c, d) =>
                new object[]
                {
                    JoinType.Inner, a.ComponentTypeId == b.Id,
                    JoinType.Inner, a.Id == c.CommodityCodeId,
                    JoinType.Inner, c.Id == d.PartNumberId
                }).Where((a, b, c, d) =>
                d.ProjectId == searchCondition.ProjectId && d.DeviceId == searchCondition.DeviceId);


            if (searchCondition.CommodityCode.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => a.Code.Contains(searchCondition.CommodityCode));

            }

            if (searchCondition.ComponentTypeCode.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => b.Code.Contains(searchCondition.ComponentTypeCode));

            }

            if (searchCondition.ComponentTypeDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => b.Desc.Contains(searchCondition.ComponentTypeDesc));

            }

            if (searchCondition.PartNumberCNDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.CN_LongDesc.Contains(searchCondition.PartNumberCNDesc));

            }

            if (searchCondition.PartNumberCode.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.Code.Contains(searchCondition.PartNumberCode));

            }

            if (searchCondition.PartNumberENDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.EN_LongDesc.Contains(searchCondition.PartNumberENDesc));

            }

            if (searchCondition.PartNumberRUDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.RU_LongDesc.Contains(searchCondition.PartNumberRUDesc));

            }
            var total = 0;

            var data = await query.Select((a, b, c, d)=>new MaterialTakeOffDetailDto{ ComponentTypeCode=b.Code,Allowance = d.Allowance,ComponentTypeDesc = b.Desc,DesignQty = d.DesignQty,PartNumberCNLongDesc = c.CN_LongDesc,PartNumberCNSizeDesc = c.CN_SizeDesc,RoundUpDigit = d.RoundUpDigit,MaterialTakeOffDetailId = d.Id}).ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);
            searchCondition.Page.RecordCount = data.Value;
            foreach (var materialTakeOffDetailDto in data.Key)
            {
                if (materialTakeOffDetailDto.RoundUpDigit.HasValue && materialTakeOffDetailDto.Allowance.HasValue)
                {
                    var d = 1 / Math.Pow(10, materialTakeOffDetailDto.RoundUpDigit.Value);
                    var oldValue = materialTakeOffDetailDto.DesignQty * materialTakeOffDetailDto.Allowance.Value;
                    materialTakeOffDetailDto.RoundUp = Math.Round(oldValue, materialTakeOffDetailDto.RoundUpDigit.Value);

                    if (materialTakeOffDetailDto.RoundUp < oldValue)
                    {
                        materialTakeOffDetailDto.RoundUp += d;
                    }

                }
            }
            return data.Key;
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="materialTakeOffDetails"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMaterialTakeOffDetail(List<MaterialTakeOffDetail> materialTakeOffDetails)
        {
            return await Task.FromResult(DbContext.UpdateRange(materialTakeOffDetails)) ;
        }
    }
}