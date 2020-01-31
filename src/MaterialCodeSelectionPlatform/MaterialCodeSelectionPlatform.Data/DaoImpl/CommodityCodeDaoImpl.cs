using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaterialCodeSelectionPlatform.Data
{
    public partial class CommodityCodeDaoImpl
    {
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public async Task<List<CommodityCodeAttribute>> GetAttributeById(string id, int languageType)
        {
            //var query = Db.Queryable<CommodityCodeAttribute>((m, v) => new object[]
            //{
            //    JoinType.Left, m.Id == v.CommodityCodeAttributeId
            //}).Where((m, v) => m.CommodityCodeId == id && v.Type == languageType).Select((m,v)=>new CommodityCodeAttribute()
            //{
            //    Id=m.Id,
            //    AttributeName =  m.AttributeName,
            //    Value = v.Value
            //});

            //return await query.ToListAsync();

            return null;
        }

        /// <summary>
        /// 获取物资编码下的采购编码以及属性
        /// </summary>
        /// <param name="id"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public async Task<List<PartNumberSizeAttribute>> GetAttributeSetList(string id, int languageType)
        {
            //var query = Db
            //    .Queryable<PartNumber, PartNumberSizeAttribute, PartNumberSizeDescription
            //    >((a, b, c) => new object[]
            //    {
            //        JoinType.Inner, a.Id == b.PartNumberId,
            //        JoinType.Inner, a.Id == b.PartNumberId
            //    });


            return null;

        }
        /// <summary>
        /// 物资编码查询列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public async Task<List<CommodityCodeDto>> GetCommodityCodeDataList(CommodityCodeSerachCondition condition)
        {
            #region sql
            /*
            SELECT b.[Desc], a.Id,a.Code,a.CN_ShortDesc,a.* FROM CommodityCode a
            LEFT JOIN ComponentType b ON a.ComponentTypeId=b.Id AND b.Status= 0
            WHERE a.Code LIKE '%%' 
            AND a.Id IN('62F59486-6CF8-42D9-8622-37C3090CEE0A')
            */
            #endregion

            var query = Db.Queryable<CommodityCode, ComponentType>((a, b) => new object[] { JoinType.Left, a.ComponentTypeId == b.Id });
            query = query.Where((a, b) => a.Status == 0);
            if (condition.Code.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b) => a.Code.Contains(condition.Code));
            }
            if (condition.CommodityCodeId != null && condition.CommodityCodeId.Count > 0)
            {
                query = query.Where((a, b) => condition.CommodityCodeId.Contains(a.Id));
            }
            query = query.OrderBy((a, b) => a.Hits, OrderByType.Desc);
            var total = 0;
            var data = await query.Select((a, b) => (new CommodityCodeDto { Id = a.Id, Desc = b.Desc, Code = a.Code, CN_ShortDesc = a.CN_ShortDesc })).ToPageListAsync(condition.Page.PageNo, condition.Page.PageSize, total);
            condition.Page.RecordCount = data.Value;
            return data.Key;
        }
        /// <summary>
        /// 获取物资编码属性
        /// </summary>
        /// <param name="id">物资编码Id</param>
        /// <returns></returns>
        public async Task<List<CommodityCodeAttribute>> GetAttributeList(string id)
        {
            #region sql
            /*
            SELECT * FROM CommodityCodeAttribute WHERE CommodityCodeId='F65F077F-623B-4325-88B1-C39CADACDC7D' AND Status=0
            */
            #endregion
            var query = Db.Queryable<CommodityCodeAttribute>();
            if (id.IsNotNullAndNotEmpty())
            {
                query = query.Where(a => a.CommodityCodeId==id && a.Status==0).OrderBy(a=>a.AttributeName);
                // 更新点击的次数
                var value = Db.Updateable<CommodityCode>().UpdateColumns(it => new { it.Hits }).ReSetValue(it => it.Hits == (it.Hits + 1)).Where(it=>it.Id==id).ExecuteCommand();

            }
            var list = await query.ToListAsync();           
            return list;
        }
        /// <summary>
        /// 选择【物资编码】的采购码
        /// </summary>
        /// <param name="commodityCodeId">物资编码Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<List<PartNumberReport>> GetCommodityCodePartNumberList(string commodityCodeId,string userId)
        {
            #region sql
            /*
           --采购码
            SELECT c.[Desc] ComponentTypeName, b.DesignQty , a. *
            FROM PartNumber a
            LEFT JOIN ComponentType c ON a.ComponentTypeId =c.Id AND c.Status=0
            LEFT JOIN (SELECT a. *
			             FROM MaterialTakeOffDetail a
			             WHERE MaterialTakeOffId = (SELECT TOP 1 Id
			                                         FROM MaterialTakeOff
			                                         WHERE status = 0
			                                             AND CreateUserId = '24271a95-c37e-4fd2-bde5-4c41cab7fb74'
			                                         ORDER BY Version DESC)
             ) b ON a.Id = b.CommodityCodeId
            WHERE a.Status = 0 AND a.CommodityCodeId = '8BE429B9-EFCB-4876-A48C-7B4BDAE6534E'
    


   
            */
            #endregion
            List<PartNumberReport> list = new List<PartNumberReport>();
            var sql = $@" SELECT c.[Desc] ComponentTypeName, b.DesignQty , a. *
            FROM PartNumber a
            LEFT JOIN ComponentType c ON a.ComponentTypeId =c.Id AND c.Status=0
            LEFT JOIN (SELECT a. *
			             FROM MaterialTakeOffDetail a
			             WHERE MaterialTakeOffId = (SELECT TOP 1 Id
			                                         FROM MaterialTakeOff
			                                         WHERE status = 0
			                                             AND CreateUserId = @CreateUserId
			                                         ORDER BY Version DESC)
             ) b ON a.id = b.PartNumberId
            WHERE a.Status = 0 AND a.CommodityCodeId = @CommodityCodeId order by b.DesignQty desc";
           var partNumberList= Db.Ado.SqlQuery<PartNumberDto>(sql, new { CreateUserId =userId, CommodityCodeId = commodityCodeId });
            //if (partNumberList != null && partNumberList.Count > 0)
            //{
            //   // var groupList = partNumberList.GroupBy(c => c.ComponentTypeId).Select(c=>new List<PartNumberReport> { ComponentTypeName=c.Key,PartNumberList=c)})
            
            //    foreach (var row in partNumberList.GroupBy(c => c.ComponentTypeName))
            //    {
            //      var ent=  list.Where(c => c.ComponentTypeName == row.Key).FirstOrDefault();
            //        if (ent == null)
            //        {
            //           // ent.PartNumberList.Add(row.)
            //        }
            //    }
            //}
            var resut =  from p in partNumberList
                        group p by p.ComponentTypeName into g
                        orderby g.Key
                        select new PartNumberReport()
                        {
                           ComponentTypeName=g.Key,
                           PartNumberList= g.ToList()
                        };
            return await Task.Run(()=>{ return resut.ToList(); }); 
        }
        /// <summary>
        /// 保存【物资汇总明细表】
        /// </summary>
        /// <param name="list">采购码列表</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetail>> SaveMaterialTakeOffDetail(List<PartNumberDto> list)
        {
            List<MaterialTakeOffDetail> listDetail = new List<MaterialTakeOffDetail>();
            if (list != null && list.Count > 0)
            {
                var partIds = list.Select(c => c.Id).ToList();
                var tempEntity = list[0];
                var projectId = tempEntity.ProjectId;
                var deviceId = tempEntity.DeviceId;
                var userId = tempEntity.CreateUserId;
                var commodityCodeId = tempEntity.CommodityCodeId;//物资编码ID
                var commodityCode = Db.Queryable<CommodityCode>().Where(c => c.Id == commodityCodeId && c.Status == 0).Single();
                if (commodityCode == null)
                {
                    throw new Exception($"找不到物资编码：{commodityCodeId}");
                }
                var mto = await getOnwerTopMaterialTakeOff(userId, projectId, deviceId);
                if (mto != null)
                {
                    var ent = Db.Deleteable<MaterialTakeOffDetail>().Where(c => c.MaterialTakeOffId == mto.Id).ExecuteCommand();//暂进不留历史记录
                    if (mto.CheckStatus == 1)//审批状态【1：working 】【2：approved】
                    {                     
                        //新增明细
                        addMaterialTakeOffDetail(list, listDetail, partIds, projectId, deviceId, userId, commodityCode, mto);
                    }
                    else
                    {
                        // 物资汇总表                   
                        mto = addMaterialTakeOff(projectId, deviceId, userId, 1, mto.Version+1);
                        //新增明细
                        addMaterialTakeOffDetail(list, listDetail, partIds, projectId, deviceId, userId, commodityCode, mto);
                    }
                }
                else
                {
                    // 物资汇总表                   
                    mto= addMaterialTakeOff(projectId, deviceId, userId,1,0);
                    //新增明细
                    addMaterialTakeOffDetail(list, listDetail, partIds, projectId, deviceId, userId, commodityCode, mto);
                }
            }           
            return listDetail;
        }
        /// <summary>
        /// 新增【物资汇总表】
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="deviceId">装置ID</param>
        /// <param name="userId">用户ID</param>
        ///  <param name="checkStatus">审批状态【1：working 】【2：approved】</param>
        ///   <param name="version">版次</param>
        private MaterialTakeOff addMaterialTakeOff(string projectId,string deviceId,string userId,int checkStatus=1,int version=0)
        {
            #region 物资汇总表
           var  mto = new MaterialTakeOff();
            mto.Id = Guid.NewGuid().ToString(); ;
            mto.CheckStatus = checkStatus;//审批状态【1：working 】【2：approved】
            mto.Version = version;
            mto.ProjectId = projectId;
            mto.DeviceId = deviceId;
            mto.Flag = 0;
            mto.Status = 0;
            mto.CreateUserId = userId;
            mto.CreateTime = DateTime.Now;
            mto.LastModifyUserId = userId;
            mto.LastModifyTime = DateTime.Now;
            var temp = Db.Insertable<MaterialTakeOff>(mto).ExecuteCommand();
            #endregion
            return mto;
        }
        private void addMaterialTakeOffDetail(List<PartNumberDto> list, List<MaterialTakeOffDetail> listDetail, List<string> partIds, string projectId, string deviceId, string userId, CommodityCode commodityCode, MaterialTakeOff mto)
        {
            var partNumberList = Db.Queryable<PartNumber>().In(partIds).ToList();
            if (partNumberList != null && partNumberList.Count > 0)
            {
                foreach (var ent in partNumberList)
                {
                    #region 物资汇总明细表
                    MaterialTakeOffDetail detail = new MaterialTakeOffDetail();
                    detail.Id = Guid.NewGuid().ToString();
                    detail.MaterialTakeOffId = mto.Id;
                    detail.CommodityCodeId = ent.CommodityCodeId;
                    detail.PartNumberId = ent.Id;//采购码Id
                    detail.CN_CommodityShortDesc = commodityCode.CN_ShortDesc;//物资编码短描述_中文
                    detail.EN_CommodityShortDesc = commodityCode.EN_ShortDesc;//物资编码短描述_英文
                    detail.RU_CommodityShortDesc = commodityCode.RU_ShortDesc;//物资编码短描述_俄文
                    detail.CN_CommodityLongDesc = commodityCode.CN_LongDesc;//物资编码长描述_中文
                    detail.EN_CommodityLongDesc = commodityCode.EN_LongDesc;//物资编码长描述_英文
                    detail.RU_CommodityLongDesc = commodityCode.RU_LongDesc;//物资编码长描述_俄文

                    detail.CN_PartNumberShortDesc = ent.CN_ShortDesc;//采购码短描述_中文
                    detail.EN_PartNumberShortDesc = ent.EN_ShortDesc;//采购码短描述_英文
                    detail.RU_PartNumberShortDesc = ent.RU_ShortDesc;//采购码短描述_俄文
                    detail.CN_PartNumberLongDesc = ent.CN_LongDesc;//采购码长描述_中文
                    detail.EN_PartNumberLongDesc = ent.EN_LongDesc;//采购码长描述_英文
                    detail.RU_PartNumberLongDesc = ent.RU_LongDesc;//采购码长描述_俄文

                    detail.CN_SizeDesc = ent.CN_SizeDesc;//尺寸描述_中文
                    detail.EN_SizeDesc = ent.EN_SizeDesc;//尺寸描述_英文
                    detail.RU_SizeDesc = ent.RU_SizeDesc;//尺寸描述_俄文
                    detail.ProjectId = projectId;//所属项目
                    detail.DeviceId = deviceId;//所属装置
                    detail.DesignQty = list.Where(c => c.Id == ent.Id).FirstOrDefault().DesignQty;//数量
                    detail.Flag = 0;
                    detail.Status = 0;
                    detail.CreateUserId = userId;
                    detail.CreateTime = DateTime.Now;
                    detail.LastModifyUserId = userId;
                    detail.LastModifyTime = DateTime.Now;
                    var temp2 = Db.Insertable<MaterialTakeOffDetail>(detail).ExecuteCommand();
                    listDetail.Add(detail);
                    #endregion
                }
            }
        }

        /// <summary>
        /// 获取自己最新的【物资汇总】记录
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="projectid">项目ID</param>
        /// <param name="deviceid">装置Id</param>
        /// <returns></returns>
        private async Task<MaterialTakeOff> getOnwerTopMaterialTakeOff(string userid,string projectid,string deviceid)
        {
            #region sql
            /*
           SELECT TOP 1 * FROM MaterialTakeOff WHERE CreateUserId='' AND ProjectId='' AND DeviceId='' AND Status=0 ORDER BY Version desc
            */
            #endregion
            var model =await Db.Queryable<MaterialTakeOff>().Where(c=>c.Status==0 && c.CreateUserId==userid && c.DeviceId==deviceid).OrderBy(c=>c.CreateTime,OrderByType.Desc).FirstAsync();
            return model;
        }
    }
}