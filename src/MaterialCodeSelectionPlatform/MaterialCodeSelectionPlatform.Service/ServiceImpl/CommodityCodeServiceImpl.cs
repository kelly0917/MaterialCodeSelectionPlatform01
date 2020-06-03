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
        /// <param name="projectId">项目ID</param>
        /// <param name="deviceId">装置Id</param>
        /// <returns></returns>
        public async Task<List<PartNumberReport>> GetCommodityCodePartNumberList(string commodityCodeId, string userId, string projectId, string deviceId)
        {
            return await _CommodityCodeDao.GetCommodityCodePartNumberList(commodityCodeId, userId, projectId, deviceId);
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
        /// 保存:物料报表【物资汇总明细表】数量
        /// </summary>
        /// <param name="detailList">MaterialTakeOffDetail集合</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetail>> UpdateReportMaterialTakeOffDetail(List<MaterialTakeOffDetail> detailList, string approver)
        {
            return await _CommodityCodeDao.UpdateReportMaterialTakeOffDetail(detailList,approver);
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
        /// <param nam/// <summary>
        /// 获取用户的物料表
        /// </summary>
        /// <param name="mtoId">mtoId</param>
        /// <param name="revision">版本</param>
        /// <param name="userId">用户Id</param>
        /// <param name="projectid">项目Id</param>
        /// <param name="deviceid">装置Id</param>
        /// <param name="downLoad">【0：查看】【1：下载】</param>
        /// <returns></returns>
        public async Task<List<PartNumberReport>> GetUserMaterialTakeReport(string mtoId, string revision, string userId, string projectid, string deviceid, int downLoad)
        {
            return await _CommodityCodeDao.GetUserMaterialTakeReport( mtoId, revision, userId, projectid, deviceid,downLoad);
        }
        /// <summary>
        /// 删除 MaterialTakeOffDetail
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public async  Task<int> DeleteById(string id)
        {
            return await _CommodityCodeDao.DeleteById(id);
        }
        /// <summary>
        /// 拷贝
        /// </summary>
        /// <param name="mtoId">选的MTO</param>
        /// <param name="userId">当前用户</param>
        ///  <param name="projectId">当前的项目ID</param>
        ///  <param name="deviceId">当前的装置ID</param>
        /// <param name="type">【0：追加拷贝】【1：覆盖拷贝】</param>
        /// <returns></returns>
        public async Task<int> CopyMaterialTakeOff(string mtoId, string userId, string projectId, string deviceId, int type)
        {
            return await _CommodityCodeDao.CopyMaterialTakeOff(mtoId, userId, projectId, deviceId, type);
        }
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="mto"></param>
        /// <returns></returns>
        public async Task<int> ApproveMto(MaterialTakeOff mto)
        {
            return await _CommodityCodeDao.ApproveMto(mto);
        }
    }
}