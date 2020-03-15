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
        /// <param name="catalogId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<List<ComponentTypeCount>> GetByCommodityCodeDesc(string catalogId, string desc)
        {
            return await _ComponentTypeDao.GetByCommodityCodeDesc(catalogId, desc);
        }

        /// <summary>
        /// 获取物资类型的物资编码获取对应的属性和属性值
        /// </summary>
        /// <param name="compenentTypeId"></param>
        /// <returns></returns>
        public async Task<List<ComAttrModel>> GetAttributeByCompenetType(string compenentTypeId)
        {
            return await _ComponentTypeDao.GetAttributeByCompenetType(compenentTypeId);
        }
    }
}