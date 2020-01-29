using System.Collections.Generic;
using System.Threading.Tasks;
using CommodityCodeSelectionPlatform.Domain;
using CommodityCodeSelectionPlatform.Domain.Entities;

namespace CommodityCodeSelectionPlatform.Service
{
    public partial interface IDeviceService
    {
        /// <summary>
        /// 搜索装置
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        Task<List<Device>> GetDataList(DeviceSearchCondition searchCondition);

        /// <summary>
        /// 验证页面元素
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        Task<bool> VerifNameOrCodeAsync(string nameCode, string projectId, string id);
    }
}