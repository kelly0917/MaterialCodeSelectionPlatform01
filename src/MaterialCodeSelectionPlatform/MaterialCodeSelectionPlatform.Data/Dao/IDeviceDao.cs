using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial interface IDeviceDao
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
        Task<bool> VerifNameOrCodeAsync(string nameCode,string projectId, string id);
    }
}