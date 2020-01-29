using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class DeviceServiceImpl
    {
        /// <summary>
        /// 搜索装置
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public async Task<List<Device>> GetDataList(DeviceSearchCondition searchCondition)
        {
            return await _DeviceDao.GetDataList(searchCondition);
        }

        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        public async Task<bool> VerifNameOrCodeAsync(string nameCode, string projectId, string id)
        {
            return await _DeviceDao.VerifNameOrCodeAsync(nameCode, projectId, id);
        }
    }
}