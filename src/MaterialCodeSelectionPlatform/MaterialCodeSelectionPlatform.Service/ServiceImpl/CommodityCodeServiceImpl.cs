using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial class CommodityCodeServiceImpl
    {
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public async Task<List<CommodityCodeAttribute>> GetAttributeById(string id, int languageType)
        {
            return await _CommodityCodeDao.GetAttributeById(id,languageType);
        }

        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public async Task<List<CommodityCodeDto>> GetCommodityCodeDataList(CommodityCodeSerachCondition condition)
        {
            return await _CommodityCodeDao.GetCommodityCodeDataList(condition);
        }
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id">物资编码Id</param>
        /// <returns></returns>
        public async Task<List<CommodityCodeAttribute>> GetAttributeList(string id)
        {
            return await _CommodityCodeDao.GetAttributeList(id);
        }
        /// <summary>
        /// 获取【物资类型】属性
        /// </summary>
        /// <param name="id">物资类型Id</param>
        /// <returns></returns>
        public async Task<List<ComponentTypeAttribute>> GetComponentTypeAttributeList(string id)
        {
            return await _CommodityCodeDao.GetComponentTypeAttributeList(id);
        }
        /// <summary>
        /// 选择【物资编码】的采购码
        /// </summary>
        /// <param name="commodityCodeId">物资编码Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<List<PartNumberReport>> GetCommodityCodePartNumberList(string commodityCodeId, string userId)
        {
            return await _CommodityCodeDao.GetCommodityCodePartNumberList(commodityCodeId, userId);
        }
        /// <summary>
        /// 保存【物资汇总明细表】
        /// </summary>
        /// <param name="condtion">条件</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetail>> SaveMaterialTakeOffDetail(PartNumberCondition condtion)
        {
            return await _CommodityCodeDao.SaveMaterialTakeOffDetail(condtion);
        }
        /// <summary>
        /// 获取用户的【物资汇总表】
        /// </summary>
        /// <param name="userid">用户Id</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDto>> GetUserMaterialTakeOff(string userid)
        {
            return await _CommodityCodeDao.GetUserMaterialTakeOff(userid);
        }
        /// <summary>
        /// 获取用户的物料表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="projectid">项目Id</param>
        /// <param name="deviceid">装置Id</param>
        /// <returns></returns>
        public async Task<List<PartNumberReport>> GetUserMaterialTakeReport(string userId, string projectid, string deviceid)
        {
            return await _CommodityCodeDao.GetUserMaterialTakeReport(userId, projectid, deviceid);
        }
    }
}