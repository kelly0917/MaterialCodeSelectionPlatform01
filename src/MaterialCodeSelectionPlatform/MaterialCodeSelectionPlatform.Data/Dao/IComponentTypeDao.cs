using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial interface IComponentTypeDao
    {
        /// <summary>
        /// 根据物资编码描述，获取对应的类型列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        Task<List<ComponentTypeCount>> GetByCommodityCodeDesc(string projectId, string desc);

    }
}