using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain.DTO;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class ComponentTypeServiceImpl
    {
        /// <summary>
        /// 根据物资编码描述，获取对应的类型列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<List<ComponentTypeCount>> GetByCommodityCodeDesc(string projectId, string desc)
        {
            return await _ComponentTypeDao.GetByCommodityCodeDesc(projectId, desc);
        }
    }
}