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
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetailDto>> GetManagerList(MaterialTakeOffDetailSearchCondition condition)
        {
            var query = Db.Queryable<CommodityCode, ComponentType, PartNumber, MaterialTakeOffDetail>((a, b, c, d) =>
                new object[]
                {
                    JoinType.Inner, a.ComponentTypeId == b.Id,
                    JoinType.Inner, a.Id == c.CommodityCodeId,
                    JoinType.Inner, c.Id == d.PartNumberId
                }).Where((a, b, c, d) =>
                d.ProjectId == condition.ProjectId && d.DeviceId == condition.DeviceId);


            if (condition.CommodityCode.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => a.Code.Contains(condition.CommodityCode));

            }

            if (condition.ComponentTypeCode.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => b.Code.Contains(condition.ComponentTypeCode));

            }

            if (condition.ComponentTypeDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => b.Desc.Contains(condition.ComponentTypeDesc));

            }

            if (condition.PartNumberCNDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.CN_LongDesc.Contains(condition.PartNumberCNDesc));

            }

            if (condition.PartNumberCode.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.Code.Contains(condition.PartNumberCode));

            }

            if (condition.PartNumberENDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.EN_LongDesc.Contains(condition.PartNumberENDesc));

            }

            if (condition.PartNumberRUDesc.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b, c, d) => c.RU_LongDesc.Contains(condition.PartNumberRUDesc));

            }

            if (string.IsNullOrEmpty(condition.OrderBy))
            {
                query = query.OrderBy((a, b, c, d) => a.Hits, OrderByType.Desc);
            }
            else
            {
                //升序
                if (condition.OrderByType == 0)
                {
                    if (condition.OrderBy.Equals("ComponentTypeCode", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => b.Code, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("Allowance", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => d.Allowance, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("ComponentTypeDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => b.Desc, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("DesignQty", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => d.DesignQty, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("PartNumberCNLongDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => c.CN_LongDesc, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("PartNumberCNSizeDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => c.CN_SizeDesc, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("RoundUpDigit", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => d.RoundUpDigit, OrderByType.Asc);
                    }
                    else
                    {
                        query = query.OrderBy((a, b, c, d) => b.Code, OrderByType.Asc);
                    }
                }
                else
                {
                    if (condition.OrderBy.Equals("ComponentTypeCode", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => b.Code, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("Allowance", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => d.Allowance, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("ComponentTypeDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => b.Desc, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("DesignQty", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => d.DesignQty, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("PartNumberCNLongDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => c.CN_LongDesc, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("PartNumberCNSizeDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => c.CN_SizeDesc, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("RoundUpDigit", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b, c, d) => d.RoundUpDigit, OrderByType.Desc);
                    }
                    else
                    {
                        query = query.OrderBy((a, b, c, d) => b.Code, OrderByType.Desc);
                    }
                }
            }
            var total = 0;

            var data = await query.Select((a, b, c, d) => new MaterialTakeOffDetailDto
            {
                ComponentTypeCode = b.Code,
                Allowance = d.Allowance,
                ComponentTypeDesc = b.Desc,
                DesignQty = d.DesignQty,
                PartNumberCNLongDesc = c.CN_LongDesc,
                PartNumberCNSizeDesc = c.CN_SizeDesc,
                RoundUpDigit = d.RoundUpDigit,
                MaterialTakeOffDetailId = d.Id
            }).ToPageListAsync(condition.Page.PageNo, condition.Page.PageSize, total);
            condition.Page.RecordCount = data.Value;


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

                if (materialTakeOffDetailDto.Allowance.HasValue)
                {
                    materialTakeOffDetailDto.AllowanceStr = (materialTakeOffDetailDto.Allowance.Value * 100) + "%";
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
            return await Task.FromResult(DbContext.UpdateRange(materialTakeOffDetails));
        }
    }
}