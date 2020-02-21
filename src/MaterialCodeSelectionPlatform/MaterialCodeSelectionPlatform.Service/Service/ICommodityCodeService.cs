using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.Service
{
    public partial interface ICommodityCodeService
    {
        /// <summary>
        /// 物资编码查询
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        Task<List<CommodityCodeDto>> GetCommodityCodeDataList(CommodityCodeSerachCondition condition);
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id">物资编码Id</param>
        /// <returns></returns>
        Task<List<CommodityCodeAttribute>> GetAttributeList(string id);
        /// <summary>
        /// 获取【物资类型】属性
        /// </summary>
        /// <param name="id">物资类型Id</param>
        /// <returns></returns>
        Task<List<ComponentTypeAttribute>> GetComponentTypeAttributeList(string id);
        /// <summary>
        /// 选择【物资编码】的采购码
        /// </summary>
        /// <param name="commodityCodeId">物资编码Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="deviceId">装置Id</param>
        /// <returns></returns>
        Task<List<PartNumberReport>> GetCommodityCodePartNumberList(string commodityCodeId, string userId, string projectId, string deviceId);
        /// <summary>
        /// 保存【物资汇总明细表】
        /// </summary>
        /// <param name="condtion">条件</param>
        /// <returns></returns>
        Task<List<MaterialTakeOffDetail>> SaveMaterialTakeOffDetail(PartNumberCondition condtion);
        /// <summary>
        /// 保存:物料报表【物资汇总明细表】数量
        /// </summary>
        /// <param name="detailList">MaterialTakeOffDetail集合</param>
        /// <returns></returns>
        Task<List<MaterialTakeOffDetail>> UpdateReportMaterialTakeOffDetail(List<MaterialTakeOffDetail> detailList);
        /// <summary>
        /// 获取用户的【物资汇总表】
        /// </summary>
        /// <param name="userid">用户Id</param>
        /// <returns></returns>
        Task<List<MaterialTakeOffDto>> GetUserMaterialTakeOff(string userid);
        /// <summary>
        /// 获取用户的物料表
        /// </summary>
        /// <param name="mtoId">mtoId</param>
        /// <param name="revision">版本</param>
        /// <param name="userId">用户Id</param>
        /// <param name="projectid">项目Id</param>
        /// <param name="deviceid">装置Id</param>
        /// <param name="downLoad">【0：查看】【1：下载】</param>
        /// <returns></returns>
        Task<List<PartNumberReport>> GetUserMaterialTakeReport(string mtoId, string revision, string userId, string projectid, string deviceid, int downLoad);
        /// <summary>
        /// 删除 MaterialTakeOffDetail
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        Task<int> DeleteById(string id);
        /// <summary>
        /// 拷贝
        /// </summary>
        /// <param name="mtoId"></param>
        /// <param name="userId"></param>
        /// <param name="type">【0：追加拷贝】【1：覆盖拷贝】</param>
        /// <returns></returns>
        Task<int> CopyMaterialTakeOff(string mtoId, string userId, int type);
    }
}