using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.DTO;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using Newtonsoft.Json;
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
        /// 物资汇总表查询(最近工作)
        /// </summary>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDto>> GetUserMaterialTakeOffList(MtoSearchCondition searchCondition)
        {
            var query = Db.Queryable<MaterialTakeOff, Project, Device, User>((a, b, c, d) => new object[] {
              JoinType.Inner,a.ProjectId==b.Id,
              JoinType.Inner,a.DeviceId==c.Id,
              JoinType.Left,a.CreateUserId==d.Id
            });
            if (!string.IsNullOrEmpty(searchCondition.UserId))
            {
                query = query.Where(a => a.CreateUserId == searchCondition.UserId || (a.Approver == searchCondition.UserId && a.CheckStatus == 1));
            }
            //if (searchCondition.Type == 0)
            //{
            //    query = query.Where(a =>a.CheckStatus==1);
            //}
            if (!string.IsNullOrEmpty(searchCondition.MtoId))
            {
                query = query.Where(a => a.Id == searchCondition.MtoId);
            }
            //1. 工作中：（approver ==null ）  
            //2. 待审批： approver !=null && status = working
            //3. 已审批： approver!= null && status =approved
            //其中待审批的，按照lastModifyTIme 顺序，越早提出的越前边。   工作中或者已审批的按照LastModifyTime 倒序，越晚提出的越前边
            var total = 0;
            var data = await query.Select((a, b, c, d) => new MaterialTakeOffDto() { ProjectName = b.Name, DeviceName = c.Name, UserName = d.Name, Id = a.Id, ProjectId = a.ProjectId, DeviceId = a.DeviceId, ApproveContent = a.ApproveContent, ApproveDate = a.ApproveDate, Approver = a.Approver, CheckStatus = a.CheckStatus, CreateTime = a.CreateTime, LastModifyTime = a.LastModifyTime, CreateUserId = a.CreateUserId, Revision = a.Revision, Version = a.Version })
                .ToPageListAsync(searchCondition.Page.PageNo, searchCondition.Page.PageSize, total);
            searchCondition.Page.RecordCount = data.Value;

            var approverList = data.Key?.Where(a => !string.IsNullOrEmpty(a.Approver) && a.CheckStatus == 1).ToList();//等待审批
            var workingList = data.Key?.Where(a => (string.IsNullOrEmpty(a.Approver) && a.CheckStatus == 1) || a.CheckStatus == 2).ToList();//已审核，工作中

            approverList = approverList?.OrderBy(c => c.CheckStatus).ThenBy(c => c.LastModifyTime).ToList();
            workingList = workingList?.OrderByDescending(c => c.LastModifyTime).ToList();

            List<MaterialTakeOffDto> newList = new List<MaterialTakeOffDto>();
            newList.AddRange(approverList);
            newList.AddRange(workingList);
            if (newList.Count > 0)
            {
                return newList;
            }
            else
            {
                return data.Key;
            }
        }
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
            List<string> commodityCodeIdList = new List<string>();



            var query = Db.Queryable<CommodityCode, ComponentType>((a, b) => new object[] { JoinType.Left, a.ComponentTypeId == b.Id });

            if (condition.InputText.IsNotNullAndNotEmpty())
            {
                if (condition.InputText == "清空列表数据")
                {
                    return new List<CommodityCodeDto>();
                }


                var descList = condition.InputText.Split(@" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in descList)
                {
                    query = query.Where((a, b) => a.CN_LongDesc.Contains(s));

                }



            }

            query = query.Where((a, b) => a.Status == 0);


            if (condition.ComponentTypeId.IsNotNullAndNotEmpty())
            {
                query = query.Where((a, b) => a.ComponentTypeId == condition.ComponentTypeId);
            }
            else
            {
                if (condition.CatelogId.IsNotNullAndNotEmpty())
                {
                    query = query.Where((a, b) => b.CatalogId == condition.CatelogId);
                }
            }

            if (condition.CompenetAttributes.Count > 0)
            {
                // SELECT DISTINCT ComponentTypeId FROM CommodityCodeAttribute WHERE Status = 0 AND AttributeName = 'Flange Rating' AND AttributeValue IN('None', 'ASTM A350 Grade LF2 Class 1')
                // var tempList = Db.Queryable<CommodityCodeAttribute>().Where(c=>c.Status==0&&c.AttributeName==condition.AttrName&&condition.AttrValue.Contains(c.AttributeValue)).GroupBy(it => new { it.ComponentTypeId}).Select(it => new { it.ComponentTypeId}).ToList();

                var attrbuteNames = condition.CompenetAttributes.Select(c => c.AttrName).Distinct().ToList();



                foreach (var attrbuteName in attrbuteNames)
                {
                    if (commodityCodeIdList.Count > 0)
                    {
                        var queryC = Db.Queryable<CommodityCodeAttribute>()
                            .Where(c => c.Status == 0 && c.ComponentTypeId == condition.ComponentTypeId);

                        var values = condition.CompenetAttributes.Where(c => c.AttrName == attrbuteName)
                            .Select(c => c.AttrValue).Distinct().ToList();

                        var list = await queryC.Where(c => c.AttributeName == attrbuteName && values.Contains(c.AttributeValue)).Select(c => c.CommodityCodeId).ToListAsync();
                        var oldList = commodityCodeIdList.ToList();
                        commodityCodeIdList = list.Where(c => oldList.Contains(c)).Distinct().ToList();
                    }
                    else
                    {
                        var queryC = Db.Queryable<CommodityCodeAttribute>()
                            .Where(c => c.Status == 0 && c.ComponentTypeId == condition.ComponentTypeId);

                        var values = condition.CompenetAttributes.Where(c => c.AttrName == attrbuteName)
                            .Select(c => c.AttrValue).Distinct().ToList();

                        var list = await queryC.Where(c => c.AttributeName == attrbuteName && values.Contains(c.AttributeValue)).Select(c => c.CommodityCodeId).ToListAsync();
                        commodityCodeIdList = list.Distinct().ToList();
                    }

                }


                //foreach (var conditionCompenetAttribute in condition.CompenetAttributes)
                //{

                //    var tempList = await queryC.Where(c =>
                //        c.AttributeName == conditionCompenetAttribute.AttrName &&
                //        conditionCompenetAttribute.AttrValue == (c.AttributeValue)).ToListAsync();
                //    if (tempList != null && tempList.Count > 0)
                //    {
                //        commodityCodeIdList.AddRange(tempList.Select(c => c.CommodityCodeId).ToList());
                //    }
                //}
                query = query.Where((a, b) => commodityCodeIdList.Contains(a.Id));
            }





            //if (condition.Code.IsNotNullAndNotEmpty())
            //{
            //    query = query.Where((a, b) => a.Code.Contains(condition.Code));
            //}
            if (string.IsNullOrEmpty(condition.OrderBy))
            {
                query = query.OrderBy((a, b) => a.Hits, OrderByType.Desc);
            }
            else
            {
                //升序
                if (condition.OrderByType == 0)
                {
                    if (condition.OrderBy.Equals("Code", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b) => a.Code, OrderByType.Asc);
                    }
                    else if (condition.OrderBy.Equals("CN_ShortDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b) => a.CN_ShortDesc, OrderByType.Asc);
                    }
                    else
                    {
                        query = query.OrderBy((a, b) => a.Hits, OrderByType.Desc);
                    }
                }
                else
                {
                    if (condition.OrderBy.Equals("Code", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b) => a.Code, OrderByType.Desc);
                    }
                    else if (condition.OrderBy.Equals("CN_ShortDesc", StringComparison.OrdinalIgnoreCase))
                    {
                        query = query.OrderBy((a, b) => a.CN_ShortDesc, OrderByType.Desc);
                    }
                    else
                    {
                        query = query.OrderBy((a, b) => a.Hits, OrderByType.Desc);
                    }
                }
            }

            var total = 0;
            var data = await query.Select((a, b) => (new CommodityCodeDto { Id = a.Id, Desc = b.Desc, Code = a.Code, CN_ShortDesc = a.CN_ShortDesc })).ToPageListAsync(condition.Page.PageNo, condition.Page.PageSize, total);
            condition.Page.RecordCount = data.Value;
            return data.Key;
        }
        /// <summary>
        /// 获取【物资编码】属性
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
                query = query.Where(a => a.CommodityCodeId == id && a.Status == 0).OrderBy(a => a.SeqNo);
                // 更新点击的次数
                var value = Db.Updateable<CommodityCode>().UpdateColumns(it => new { it.Hits }).ReSetValue(it => it.Hits == (it.Hits + 1)).Where(it => it.Id == id).ExecuteCommand();

            }
            var list = await query.ToListAsync();
            return list;
        }
        /// <summary>
        /// 获取【物资类型】属性
        /// </summary>
        /// <param name="id">物资类型Id</param>
        /// <returns></returns>
        public async Task<List<ComponentTypeAttribute>> GetComponentTypeAttributeList(string id)
        {
            #region sql
            /*
            SELECT a.Id,a.AttributeName,a.AttributeValue FROM CommodityCodeAttribute a WHERE a.Status=0 AND a.ComponentTypeId='031418E4-D05A-43A1-B87C-3E1C1AD8F3C2' AND a.AttributeValue!='' ORDER BY a.AttributeName
            */
            #endregion
            List<ComponentTypeAttribute> attrList = new List<ComponentTypeAttribute>();
            var list = await Db.Queryable<CommodityCodeAttribute>().Where(c => c.Status == 0 && c.AttributeValue != "" && c.ComponentTypeId == id).ToListAsync();
            if (list != null && list.Count > 0)
            {
                var resut = from p in list
                            group p by p.AttributeName into g
                            orderby g.Key
                            select new ComponentTypeAttribute()
                            {
                                AttributeName = g.Key,
                                ValueList = g.Select(c => c.AttributeValue).Distinct().ToList()
                            };
                attrList = resut.ToList();
            }
            return attrList;
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
			                                                     AND ProjectId='a2c18936-9668-4d33-a714-d036af548545'
			                                                     AND DeviceId='9501822a-8da4-42ca-97a2-b9940ec85407'
			                                                 ORDER BY Version DESC)
                     ) b ON a.id = b.PartNumberId
                 WHERE a.Status = 0 AND a.CommodityCodeId = '0003E333-8C94-4774-98C5-855619A4C0E8' order by a.CN_SizeDesc ASC
            */
            #endregion
            List<PartNumberReport> list = new List<PartNumberReport>();
            var sql = $@" SELECT c.[Desc] ComponentTypeName,c.Unit T_Unit, b.DesignQty ,a.id, a.Code P_Code
            ,a.CN_ShortDesc P_CN_ShortDesc
            ,a.EN_ShortDesc P_EN_ShortDesc
            ,a.RU_ShortDesc P_RU_ShortDesc
            ,a.CN_LongDesc P_CN_LongDesc
            ,a.EN_LongDesc P_EN_LongDesc
            ,a.RU_LongDesc P_RU_LongDesc
            ,a.EN_SizeDesc P_CN_SizeDesc
            ,a.EN_SizeDesc P_EN_SizeDesc
            ,a.RU_SizeDesc P_RU_SizeDesc
            ,d.CN_ShortDesc C_CN_ShortDesc
            FROM PartNumber a
            LEFT JOIN ComponentType c ON a.ComponentTypeId =c.Id AND c.Status=0
            LEFT JOIN CommodityCode d ON d.Id=a.CommodityCodeId
            LEFT JOIN (SELECT DISTINCT a.DesignQty,a.PartNumberId
			             FROM MaterialTakeOffDetail a
			             WHERE MaterialTakeOffId = (SELECT TOP 1 Id
			                                         FROM MaterialTakeOff
			                                         WHERE status = 0
			                                             AND CreateUserId = @CreateUserId
 			                                             AND ProjectId=@projectId
			                                             AND DeviceId=@deviceId
			                                         ORDER BY Version DESC)
             ) b ON a.id = b.PartNumberId
            WHERE a.Status = 0 AND a.CommodityCodeId = @CommodityCodeId order by a.CN_SizeDesc asc";
            var partNumberList = Db.Ado.SqlQuery<PartNumberReportDetail>(sql, new { CreateUserId = userId, CommodityCodeId = commodityCodeId, projectId = projectId, deviceId = deviceId });
            var resut = from p in partNumberList
                        group p by p.ComponentTypeName into g
                        orderby g.Key
                        select new PartNumberReport()
                        {
                            ComponentTypeName = g.Key,
                            PartNumberReportDetailList = g.ToList()
                        };
            return await Task.Run(() => { return resut.ToList(); });
        }
        /// <summary>
        /// 保存【物资汇总明细表】
        /// </summary>
        /// <param name="condtion">条件</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetail>> SaveMaterialTakeOffDetail(PartNumberCondition condtion)
        {
            List<MaterialTakeOffDetail> listDetail = new List<MaterialTakeOffDetail>();
            var projectId = condtion.ProjectId;
            var deviceId = condtion.DeviceId;
            var userId = condtion.UserId;
            var commodityCodeId = condtion.CommodityCodeId;
            var commodityCode = Db.Queryable<CommodityCode>().Where(c => c.Id == commodityCodeId && c.Status == 0).Single();
            var mto = await getOnwerTopMaterialTakeOff(userId, projectId, deviceId);
            if (condtion.PartNumberDtoList != null && condtion.PartNumberDtoList.Count > 0)
            {
                var partIds = condtion.PartNumberDtoList.Select(c => c.Id).ToList();
                if (mto != null)
                {
                    #region 获取原有的记录
                    var entList = new { Id = "", DesignQty = 0 };
                    var oldList = Db.Queryable<MaterialTakeOffDetail>().Where(c => c.MaterialTakeOffId == mto.Id && c.CommodityCodeId == commodityCodeId);
                    var sList = oldList.Select(c => new { Id = c.PartNumberId, DesignQty = c.DesignQty }).ToList();
                    var tList = condtion.PartNumberDtoList.Select(c => new { Id = c.Id, DesignQty = c.DesignQty }).ToList();
                    #endregion
                    if (sList.SequenceEqual(tList))
                    {
                        return listDetail;//如果相同，直接返回
                    }
                    else
                    {
                        mto.Version = mto.Version + 1;
                        mto.CheckStatus = 1;
                        mto.ApproveContent = null;
                        mto.Approver = null;
                        Db.Updateable(mto).ExecuteCommand();
                        var ent = Db.Deleteable<MaterialTakeOffDetail>().Where(c => c.MaterialTakeOffId == mto.Id && c.CommodityCodeId == commodityCodeId).ExecuteCommand();//暂时不留历史记录
                                                                                                                                                                            //新增明细（保持最新）
                        addMaterialTakeOffDetail(condtion.PartNumberDtoList, listDetail, partIds, projectId, deviceId, userId, commodityCode, mto);
                    }
                }
                else
                {
                    // 物资汇总表                   
                    mto = addMaterialTakeOff(projectId, deviceId, userId, 1, 0);
                    //新增明细
                    addMaterialTakeOffDetail(condtion.PartNumberDtoList, listDetail, partIds, projectId, deviceId, userId, commodityCode, mto);
                }
            }
            else
            {
                //if (mto != null)
                //{
                //    var ent = Db.Deleteable<MaterialTakeOffDetail>().Where(c => c.MaterialTakeOffId == mto.Id && c.CommodityCodeId == commodityCodeId).ExecuteCommand();//暂时不留历史记录
                //}
            }
            return listDetail;
        }
        /// <summary>
        /// 保存:物料报表【物资汇总明细表】数量
        /// </summary>
        /// <param name="detailList">MaterialTakeOffDetail集合</param>
        /// <param name="approver">MaterialTakeOffDetail集合</param>
        /// <param name="type">【0:保存】【1：发送审批人】</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDetail>> UpdateReportMaterialTakeOffDetail(List<MaterialTakeOffDetail> detailList, string approver, int type)
        {

            if (detailList != null && detailList.Count > 0)
            {
                var mtoId = detailList.FirstOrDefault().MaterialTakeOffId;
                var ids = detailList.Select(c => c.Id).ToList();
                var mto = await Db.Queryable<MaterialTakeOff>().Where(c => c.Status == 0 && c.Id == mtoId).FirstAsync();
                bool change = false;
                var dbList = await Db.Queryable<MaterialTakeOffDetail>().Where(c => c.Status == 0 && c.MaterialTakeOffId == mtoId).ToListAsync();
                foreach (var ent in detailList)
                {
                    var model = dbList.Where(c => c.Id == ent.Id).SingleOrDefault();
                    if (model != null)
                    {
                        if (ent.DesignQty == 0)
                        {
                            change = true;
                            await Db.Deleteable<MaterialTakeOffDetail>(model).ExecuteCommandAsync();//如果是0就删除
                        }
                        else
                        {
                            if (model.DesignQty != ent.DesignQty)
                            {
                                change = true;
                            }
                            model.DesignQty = ent.DesignQty;
                            Db.Updateable<MaterialTakeOffDetail>(model).ExecuteCommand();
                        }
                    }
                    else
                    {
                        await Db.Insertable<MaterialTakeOffDetail>(ent).ExecuteCommandAsync();
                    }
                }
                #region 更新 MaterialTakeOff
                if (!string.IsNullOrEmpty(approver) && type == 1)
                {//审批
                    mto.Approver = approver;
                    mto.CheckStatus = 1;
                    mto.Version = mto.Version + 1;//当文件提交审批后，内部版次+1。并生成一次Json。也就是版次现在实际上是等于提交审批的次数。（生成报表等其他的行为都不会使版次+1。）
                }
                if (change && type == 0)
                {//保存
                    mto.CheckStatus = 1;
                    mto.Approver = "";
                    mto.ApproveContent = "";
                    mto.ApproveDate = null;
                }
                Db.Updateable<MaterialTakeOff>(mto).ExecuteCommand();
                #endregion
            }
            return detailList;
        }
        /// <summary>
        /// 新增【物资汇总表】
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="deviceId">装置ID</param>
        /// <param name="userId">用户ID</param>
        ///  <param name="checkStatus">审批状态【1：working 】【2：approved】</param>
        ///   <param name="version">版次</param>
        private MaterialTakeOff addMaterialTakeOff(string projectId, string deviceId, string userId, int checkStatus = 1, int version = 0)
        {
            #region 物资汇总表
            var mto = new MaterialTakeOff();
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
            var value = Db.Updateable<CommodityCode>().UpdateColumns(it => new { it.Hits }).ReSetValue(it => it.Hits == (it.Hits + 1)).Where(it => it.Id == commodityCode.Id).ExecuteCommand();
            var partNumberList = Db.Queryable<PartNumber>().In(partIds).ToList();
            var materialTakeOffDetailList = Db.Queryable<MaterialTakeOffDetail>().Where(c => partIds.Contains(c.PartNumberId)).ToList();
            if (partNumberList != null && partNumberList.Count > 0)
            {
                foreach (var ent in partNumberList)
                {
                    var detail = materialTakeOffDetailList.Where(c => c.MaterialTakeOffId == mto.Id && c.CommodityCodeId == ent.CommodityCodeId && c.PartNumberId == ent.Id && c.ProjectId == projectId && c.DeviceId == deviceId).FirstOrDefault();
                    if (detail != null)
                    {
                        detail.DesignQty = list.Where(c => c.Id == ent.Id).FirstOrDefault().DesignQty;//数量
                        var temp2 = Db.Updateable<MaterialTakeOffDetail>(detail).ExecuteCommand();
                    }
                    else
                    {
                        #region 物资汇总明细表
                        detail = new MaterialTakeOffDetail();
                        detail.Id = Guid.NewGuid().ToString();
                        detail.MaterialTakeOffId = mto.Id;
                        detail.CommodityCodeId = ent.CommodityCodeId;
                        detail.PartNumberId = ent.Id;//采购码Id
                                                     //detail.CN_CommodityShortDesc = commodityCode.CN_ShortDesc;//物资编码短描述_中文
                                                     //detail.EN_CommodityShortDesc = commodityCode.EN_ShortDesc;//物资编码短描述_英文
                                                     //detail.RU_CommodityShortDesc = commodityCode.RU_ShortDesc;//物资编码短描述_俄文
                                                     //detail.CN_CommodityLongDesc = commodityCode.CN_LongDesc;//物资编码长描述_中文
                                                     //detail.EN_CommodityLongDesc = commodityCode.EN_LongDesc;//物资编码长描述_英文
                                                     //detail.RU_CommodityLongDesc = commodityCode.RU_LongDesc;//物资编码长描述_俄文

                        //detail.CN_PartNumberShortDesc = ent.CN_ShortDesc;//采购码短描述_中文
                        //detail.EN_PartNumberShortDesc = ent.EN_ShortDesc;//采购码短描述_英文
                        //detail.RU_PartNumberShortDesc = ent.RU_ShortDesc;//采购码短描述_俄文
                        //detail.CN_PartNumberLongDesc = ent.CN_LongDesc;//采购码长描述_中文
                        //detail.EN_PartNumberLongDesc = ent.EN_LongDesc;//采购码长描述_英文
                        //detail.RU_PartNumberLongDesc = ent.RU_LongDesc;//采购码长描述_俄文

                        //detail.CN_SizeDesc = ent.CN_SizeDesc;//尺寸描述_中文
                        //detail.EN_SizeDesc = ent.EN_SizeDesc;//尺寸描述_英文
                        //detail.RU_SizeDesc = ent.RU_SizeDesc;//尺寸描述_俄文
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
                        #endregion
                    }
                    listDetail.Add(detail);
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
        private async Task<MaterialTakeOff> getOnwerTopMaterialTakeOff(string userid, string projectid, string deviceid)
        {
            #region sql
            /*
           SELECT TOP 1 * FROM MaterialTakeOff WHERE CreateUserId='' AND ProjectId='' AND DeviceId='' AND Status=0 ORDER BY Version desc
            */
            #endregion
            var model = await Db.Queryable<MaterialTakeOff>().Where(c => c.Status == 0 && c.CreateUserId == userid && c.ProjectId == projectid && c.DeviceId == deviceid).OrderBy(c => c.LastModifyTime, OrderByType.Desc).FirstAsync();
            return model;
        }
        /// <summary>
        /// 获取用户的【物资汇总表】
        /// </summary>
        /// <param name="userid">用户Id</param>
        /// <returns></returns>
        public async Task<List<MaterialTakeOffDto>> GetUserMaterialTakeOff(string userid, string mtoId = "")
        {
            #region SQL 
            /*
           
            SELECT
            b.Name ProjectName,c.Name DeviceName,a.*
            FROM MaterialTakeOff a
            INNER JOIN   (
                    SELECT
                        a.ProjectId,a.DeviceId,
                        MAX (a.LastModifyTime) LastModifyTime
                    FROM
                        MaterialTakeOff a
                    WHERE
                      a.Status=0 and  a.CreateUserId='24271a95-c37e-4fd2-bde5-4c41cab7fb74'
                    GROUP BY
                       a.ProjectId,a.DeviceId
                ) k on a.LastModifyTime = k.LastModifyTime
            INNER JOIN Project b ON b.Id=a.ProjectId
            INNER JOIN device  c ON c.Id=a.DeviceId
            WHERE a.Status=0 and a.CreateUserId='24271a95-c37e-4fd2-bde5-4c41cab7fb74'


           */
            #endregion
            //var sql = $@"SELECT
            //b.Name ProjectName,c.Name DeviceName,a.*
            //FROM MaterialTakeOff a
            //INNER JOIN   (
            //        SELECT
            //            a.ProjectId,a.DeviceId,
            //            MAX (a.LastModifyTime) LastModifyTime
            //        FROM
            //            MaterialTakeOff a
            //        WHERE
            //          a.Status=0 and  a.CreateUserId=@CreateUserId
            //        GROUP BY
            //           a.ProjectId,a.DeviceId
            //    ) k on a.LastModifyTime = k.LastModifyTime
            //INNER JOIN Project b ON b.Id=a.ProjectId
            //INNER JOIN device  c ON c.Id=a.DeviceId
            //WHERE a.Status=0 and a.CreateUserId=@CreateUserId";
            string where = string.Empty;
            if (!string.IsNullOrEmpty(userid))
            {
                where = " and (a.CreateUserId=@UserId or Approver= @UserId)";
            }
            if (!string.IsNullOrEmpty(mtoId))
            {
                where = " and (a.id=@mtoId )";
            }
            var sql = $@"SELECT
                            b.Name ProjectName,c.Name DeviceName,d.Name UserName,a.*
                            FROM MaterialTakeOff a            
                            INNER JOIN Project b ON b.Id=a.ProjectId
                            INNER JOIN device  c ON c.Id=a.DeviceId
                            LEFT JOIN [User] d ON d.Id=a.CreateUserId AND d.Status=0
                            WHERE a.Status=0 AND b.Status=0 AND c.Status=0 {where} ORDER BY a.LastModifyTime desc";
            var list = Db.Ado.SqlQuery<MaterialTakeOffDto>(sql, new { UserId = userid, mtoId = mtoId });
            return list;
        }
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
        public async Task<List<PartNumberReport>> GetUserMaterialTakeReport(string mtoId, string revision, string userId, string projectid, string deviceid, int downLoad)
        {
            #region SQL 
            /*
          
            SELECT d.[Desc] ComponentTypeName,c.Code,a.CN_PartNumberLongDesc, a.id from MaterialTakeOffDetail a
            INNER JOIN PartNumber c ON c.Id=a.PartNumberId
            INNER JOIN ComponentType d ON d.Id=c.ComponentTypeId
            INNER JOIN CommodityCode e ON e.Id=a.CommodityCodeId
            WHERE a.Status=0 AND c.Status=0 AND d.Status=0 AND e.status=0 AND a.MaterialTakeOffId=
            (
            SELECT TOP 1 id FROM MaterialTakeOff b WHERE b.Status=0 AND  b.ProjectId='B86AE930-3A1B-46BB-B0EF-2BBF3C81BD05' AND b.DeviceId='947386BB-68CD-4B2D-B7ED-96D281E7DB2E' AND b.CreateUserId='24271a95-c37e-4fd2-bde5-4c41cab7fb74' ORDER BY b.LastModifyTime desc
            ) ORDER BY e.code,c.code

           */
            #endregion
            //var topWhere = " ORDER BY b.LastModifyTime desc";
            MaterialTakeOff mtoEntity = null;
            //if (!string.IsNullOrWhiteSpace(mtoId))
            //{
            //    topWhere = $" and b.id='{mtoId}'";
            //    mtoEntity = await Db.Queryable<MaterialTakeOff>().Where(c => c.Id == mtoId).SingleAsync();
            //}
            //else
            //{
            //    mtoEntity = await Db.Queryable<MaterialTakeOff>().Where(c => c.Status == 0 && c.CreateUserId == userId && c.ProjectId == projectid && c.DeviceId == deviceid).OrderBy(c => c.LastModifyTime, OrderByType.Desc).FirstAsync();
            //}
            mtoEntity = await Db.Queryable<MaterialTakeOff>().Where(c => c.Status == 0 && (c.CreateUserId == userId || c.Approver == userId) && c.ProjectId == projectid && c.DeviceId == deviceid).OrderBy(c => c.LastModifyTime, OrderByType.Desc).FirstAsync();

            var sql = $@" SELECT c.Code P_Code
                               ,c.Code P_Code
                                ,c.CN_ShortDesc P_CN_ShortDesc
                                ,c.EN_ShortDesc P_EN_ShortDesc
                                ,c.RU_ShortDesc P_RU_ShortDesc
                                ,c.CN_LongDesc P_CN_LongDesc
                                ,c.EN_LongDesc P_EN_LongDesc
                                ,c.RU_LongDesc P_RU_LongDesc
                                ,c.EN_SizeDesc P_CN_SizeDesc
                                ,c.EN_SizeDesc P_EN_SizeDesc
                                ,c.RU_SizeDesc P_RU_SizeDesc
                                , e.Code          C_Code
                                , e.CN_ShortDesc  C_CN_ShortDesc
                                , e.EN_ShortDesc  C_EN_ShortDesc
                                , e.RU_ShortDesc  C_RU_ShortDesc
                                , e.CN_LongDesc   C_CN_LongDesc
                                , e.EN_LongDesc   C_EN_LongDesc
                                , e.RU_LongDesc   C_RU_LongDesc
                                , d.Code           T_Code
                                , d.[Desc]         T_Desc
                                , d.Unit           T_Unit
                                , d.Discipline     T_Discipline
                                ,d.[Desc] AS ComponentTypeName
                                ,a.*  from MaterialTakeOffDetail a
                                INNER JOIN PartNumber c ON c.Id=a.PartNumberId
                                INNER JOIN ComponentType d ON d.Id=c.ComponentTypeId
                                INNER JOIN CommodityCode e ON e.Id=a.CommodityCodeId
                                WHERE a.Status=0 AND c.Status=0 AND d.Status=0 AND e.status=0 AND a.MaterialTakeOffId=@MaterialTakeOffId ORDER BY e.code,c.code ";
            var partNumberList = Db.Ado.SqlQuery<PartNumberReportDetail>(sql, new { MaterialTakeOffId = mtoEntity.Id });
            if (partNumberList != null && partNumberList.Count > 0)
            {
                foreach (var part in partNumberList)
                {
                    part.AllowanceQty = Utilities.ConvertHelper.ConvertFloatPointData(part.DesignQty, part.Allowance, part.RoundUpDigit);
                    //   getAllowanceQty(part.RoundUpDigit, part.Allowance, part.DesignQty);
                }
            }
            var resut = from p in partNumberList
                        group p by p.ComponentTypeName into g
                        orderby g.Key
                        select new PartNumberReport()
                        {
                            ComponentTypeName = g.Key,
                            PartNumberReportDetailList = g.ToList()
                        };
            #region 项目、装置信息
            var resutList = resut?.ToList();
            if (resutList != null && resutList.Count() > 0)
            {
                var project = Db.Queryable<Project>().Where(c => c.Status == 0 && c.Id == projectid).Single();
                var device = Db.Queryable<Device>().Where(c => c.Status == 0 && c.Id == deviceid).Single();
                var user = Db.Queryable<User>().Where(c => c.Status == 0 && c.Id == userId).Single();
                var approver = Db.Queryable<User>().Where(c => c.Status == 0 && c.Id == mtoEntity.Approver).Single();
                resutList.ForEach(c =>
                {
                    c.ProjectName = project?.Name;
                    c.ProjectCode = project?.Code;
                    c.DeviceName = device?.Name;
                    c.DeviceCode = device?.Code;
                    // c.Revision = string.IsNullOrEmpty(revision)?mtoEntity.Revision :revision;
                    c.Revision = revision;
                    c.Version = mtoEntity.Version;
                    c.DeviceRemark = device?.Remark;
                    c.UserName = user?.Name;
                    c.Approver = approver?.Name;
                    c.ApproveContent = mtoEntity.ApproveContent;
                    c.DateTime = DateTime.Now.ToString("yyyy-MM-dd");

                });
            }
            #endregion
            if (downLoad == 1)
            {
                // 更新版次
                var ent = await Db.Queryable<MaterialTakeOff>().Where(it => it.Status == 0 && it.ProjectId == projectid && it.DeviceId == deviceid && (it.CreateUserId == userId || it.Approver == userId)).OrderBy(it => it.LastModifyTime, OrderByType.Desc).FirstAsync();
                if (ent != null)
                {
                    //if (ent.CheckStatus == 1)
                    //{
                    //    ent.CheckStatus = 2;
                    //}
                    ent.LastModifyTime = DateTime.Now;
                    // ent.Version = ent.Version + 1;
                    ent.Revision = revision;
                    Db.Updateable(ent).ExecuteCommand();
                }
            }
            return await Task.Run(() => { return resutList; });
        }


        /// <summary>
        /// 删除 MaterialTakeOffDetail
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public async Task<int> DeleteById(string id)
        {
            var n = await Db.Deleteable<MaterialTakeOffDetail>().Where(c => c.Id == id).ExecuteCommandAsync();
            return n;
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
            var num = 0;
            var dbMto = await Db.Queryable<MaterialTakeOff>().Where(c => c.Status == 0 && c.Id == mtoId).SingleAsync();
            var dbMtoDetailList = await Db.Queryable<MaterialTakeOffDetail>().Where(c => c.Status == 0 && c.MaterialTakeOffId == mtoId).ToListAsync();

            var ownMto = await Db.Queryable<MaterialTakeOff>().Where(c => c.Status == 0 && c.CreateUserId == userId && c.ProjectId == projectId && c.DeviceId == deviceId).SingleAsync();
            if (ownMto == null)
            {
                #region 全新
                dbMto.Id = Guid.NewGuid().ToString();
                dbMto.ProjectId = projectId;
                dbMto.DeviceId = deviceId;
                dbMto.CreateTime = DateTime.Now;
                dbMto.LastModifyTime = DateTime.Now;
                dbMto.CreateUserId = userId;
                dbMto.LastModifyUserId = userId;
                dbMto.CheckStatus = 1;
                num += await Db.Insertable(dbMto).ExecuteCommandAsync();
                if (dbMtoDetailList != null && dbMtoDetailList.Count > 0)
                {
                    foreach (var ent in dbMtoDetailList)
                    {
                        ent.Id = Guid.NewGuid().ToString();
                        ent.MaterialTakeOffId = dbMto.Id;
                        ent.ProjectId = projectId;
                        ent.DeviceId = deviceId;
                        ent.CreateTime = DateTime.Now;
                        ent.LastModifyTime = DateTime.Now;
                        ent.CreateUserId = userId;
                        ent.LastModifyUserId = userId;
                    }
                    num += await Db.Insertable(dbMtoDetailList.ToArray()).ExecuteCommandAsync();
                }

                #endregion
            }
            else
            {
                if (type == 1)
                {
                    #region 覆盖拷贝
                    num += await Db.Deleteable<MaterialTakeOffDetail>().Where(c => c.MaterialTakeOffId == ownMto.Id).ExecuteCommandAsync();
                    if (dbMtoDetailList != null && dbMtoDetailList.Count > 0)
                    {
                        foreach (var ent in dbMtoDetailList)
                        {
                            ent.Id = Guid.NewGuid().ToString();
                            ent.MaterialTakeOffId = ownMto.Id;
                            ent.ProjectId = projectId;
                            ent.DeviceId = deviceId;
                            ent.CreateTime = DateTime.Now;
                            ent.LastModifyTime = DateTime.Now;
                            ent.CreateUserId = userId;
                            ent.LastModifyUserId = userId;
                        }
                        num += await Db.Insertable(dbMtoDetailList.ToArray()).ExecuteCommandAsync();
                    }
                    #endregion
                }
                else
                {
                    if (dbMtoDetailList != null && dbMtoDetailList.Count > 0)
                    {
                        var ownDetailList = await Db.Queryable<MaterialTakeOffDetail>().Where(c => c.Status == 0 && c.MaterialTakeOffId == ownMto.Id).ToListAsync();
                        if (ownDetailList != null && ownDetailList.Count > 0)
                        {
                            #region 如果自己有明细，那就追加或叠加数量
                            foreach (var ent in dbMtoDetailList)
                            {
                                var newEntity = ownDetailList.Where(c => c.Status == 0 && c.PartNumberId == ent.PartNumberId).FirstOrDefault();
                                if (newEntity != null)
                                {
                                    newEntity.DesignQty = newEntity.DesignQty + ent.DesignQty;//叠加数量
                                    num += Db.Updateable(newEntity).ExecuteCommand();//更新
                                }
                                else
                                {
                                    ent.Id = Guid.NewGuid().ToString();
                                    ent.MaterialTakeOffId = ownMto.Id;
                                    ent.ProjectId = projectId;
                                    ent.DeviceId = deviceId;
                                    ent.CreateTime = DateTime.Now;
                                    ent.LastModifyTime = DateTime.Now;
                                    ent.CreateUserId = userId;
                                    ent.LastModifyUserId = userId;
                                    num += await Db.Insertable(ent).ExecuteCommandAsync();//追加
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 自己没有明细，直拉追加
                            foreach (var ent in dbMtoDetailList)
                            {
                                ent.Id = Guid.NewGuid().ToString();
                                ent.MaterialTakeOffId = ownMto.Id;
                                ent.ProjectId = projectId;
                                ent.DeviceId = deviceId;
                                ent.CreateTime = DateTime.Now;
                                ent.LastModifyTime = DateTime.Now;
                                ent.CreateUserId = userId;
                                ent.LastModifyUserId = userId;
                            }
                            num += await Db.Insertable(dbMtoDetailList.ToArray()).ExecuteCommandAsync();
                            #endregion
                        }
                    }
                }
            }
            return num;
        }
        /// <summary>
        /// 审批
        /// </summary>
        /// <param name="mto"></param>
        /// <returns></returns>
        public async Task<int> ApproveMto(MaterialTakeOff mto)
        {
            if (mto.CheckStatus == 1)
            {
                mto.Approver = "";//审批不通过，清空
                                  //   mto.ApproveContent = "";//审批不通过，清空               
            }
            var n = await Db.Updateable<MaterialTakeOff>().UpdateColumns(it => new MaterialTakeOff() { CheckStatus = mto.CheckStatus, Revision = mto.Revision, Approver = mto.Approver, ApproveContent = mto.ApproveContent, ApproveDate = DateTime.Now }).Where(t => t.Id == mto.Id).ExecuteCommandAsync();
            return n;
        }
        private double? getAllowanceQty(int? roundUpDigit, double? allowance, double designQty)
        {
            double? roundUp;
            if (allowance == null || roundUpDigit == null)
            {
                roundUp = 0;
            }
            else
            {
                var d = 1 / Math.Pow(10, roundUpDigit.Value);
                var oldValue = designQty * allowance.Value;
                roundUp = Math.Round(oldValue, roundUpDigit.Value);

                if (roundUp < oldValue)
                {
                    roundUp += d;
                }
            }


            return roundUp;
        }
        /// <summary>
        /// CSV文件导入
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public async Task<MaterialTakeOffDetailCSVList> ImportData(MaterialTakeOffDetailCSVList model)
        {
            /*
             
            select b.CatalogId from Project a 
            inner join ProjectCatalogMap b on b.ProjectId=a.Id
            where a.status=0 and b.Status=0 and a.code='塔里木'

            =======================

            select * from ProjectCatalogMap where Status=0 and CatalogId in(select c.CatalogId from PartNumber a 
            inner join CommodityCode b on b.Id=a.CommodityCodeId
            inner join ComponentType c on c.Id=a.ComponentTypeId
            inner join Catalog d on d.Id=c.CatalogId
            where a.Status=0 and b.Status=0 and c.Status=0 and d.Status=0 and a.code='PFT000005'
            )
             */
            try
            {
               
                var newModel =JsonConvert.DeserializeObject<MaterialTakeOffDetailCSVList>(model.SerializeToString());
                newModel.CSVList = new List<MaterialTakeOffDetailCSV>();
                foreach (var csv in model.CSVList)
                {
                    var temp = newModel.CSVList.SingleOrDefault(c => c.ProjectCode == csv.ProjectCode && c.DeviceCode == csv.DeviceCode);
                    if (temp == null)
                    {
                        newModel.CSVList.Add(csv);
                    }
                    else
                    {                       
                        foreach (var ent in csv.PartNumberDesignQty)
                        {
                            if (temp.PartNumberDesignQty.ContainsKey(ent.Key))
                            {
                                temp.PartNumberDesignQty[ent.Key] += Convert.ToDouble(ent.Value.ToString());
                            }
                           
                        }
                    }
                }
                List<MaterialTakeOffDetail> newList = new List<MaterialTakeOffDetail>();//新增的
                List<MaterialTakeOffDetail> editList = new List<MaterialTakeOffDetail>();//修改的

                model = newModel;
                foreach (var csv in model.CSVList)
                {
                    #region 判断选择的项目是否存在
                    var projectObj = await Db.Queryable<Project>().SingleAsync(c => c.Status == 0 && c.Id == csv.ProjectId);//选择的项目
                    var deviceObj = await Db.Queryable<Device>().SingleAsync(c => c.Status == 0 && c.Id == csv.DeviceId);//选择的装置
                    if (projectObj == null)
                    {
                        csv.ErrorMsg += $"#项目为空：ProjectId={csv.ProjectId}";
                        csv.LogMsg += $"#项目为空：ProjectId={csv.ProjectId}";
                        continue;
                    }
                    if (deviceObj == null)
                    {
                        csv.ErrorMsg += $"#装置为空：DeviceId={csv.DeviceId}";
                        csv.LogMsg += $"#装置为空：DeviceId={csv.DeviceId}";
                        continue;
                    }
                    if (projectObj != null && projectObj.Code.ToLower().Trim() != csv.ProjectCode.ToLower().Trim())
                    {
                        csv.ErrorMsg += $"#选择的项目【{projectObj.Code}】与CSV的项目【{csv.ProjectCode}】不一致";
                        csv.LogMsg += $"#选择的项目【{projectObj.Code}】与CSV的项目【{csv.ProjectCode}】不一致";
                        continue;
                    }
                    if (deviceObj != null && deviceObj.Code.ToLower().Trim() != csv.DeviceCode.ToLower().Trim())
                    {
                        csv.ErrorMsg += $"#选择的装置【{deviceObj.Code}】与CSV的装置【{csv.DeviceCode}】不一致";
                        csv.LogMsg += $"#选择的装置【{deviceObj.Code}】与CSV的装置【{csv.DeviceCode}】不一致";
                        continue;
                    }
                    #endregion
                    var sql = $@" select b.* from Project a  inner join ProjectCatalogMap b on b.ProjectId=a.Id  where a.status=0 and b.Status=0 and b.ProjectId=@ProjectId";//项目
                    csv.LogMsg += $"#【SQL】{sql.Replace("@ProjectId", $"'{ csv.ProjectId}'")}";
                    var mapList = Db.Ado.SqlQuery<ProjectCatalogMap>(sql, new { ProjectId = csv.ProjectId });//找出项目的所有映射关系
                    


                    if (mapList != null && mapList.Count > 0)
                    {
                        var projectId = projectObj.Id;
                        var deviceId = deviceObj.Id;
                        csv.LogMsg += $"#【1】找到项目ProjectId=[{csv.ProjectId} 与 CatalogId=[{string.Join(',', mapList.Select(c => c.CatalogId))}] 关联 ";
                        if (csv.PartNumberDesignQty != null && csv.PartNumberDesignQty.Count > 0)
                        {   
                            // 物资汇总表
                            var mto = await Db.Queryable<MaterialTakeOff>().FirstAsync(c => c.Status == 0 && c.ProjectId == projectId && c.DeviceId == deviceId && c.CreateUserId == csv.UserId);
                            if (mto == null)
                            {
                                mto = addMaterialTakeOff(projectId, deviceId, csv.UserId, 1, 0);
                                csv.LogMsg += $"#【3】新增 MaterialTakeOff：ProjectId={projectId}，deviceId={deviceId}，CreateUserId ={ csv.UserId} 成功";
                            }
                            else
                            {
                                csv.LogMsg += $"#【3】已存在 MaterialTakeOff：ProjectId={projectId}，deviceId={deviceId}，CreateUserId ={ csv.UserId} ";
                            }
                            List<MaterialTakeOffDetail> listDetail = new List<MaterialTakeOffDetail>();
                            foreach (var part in csv.PartNumberDesignQty)
                            {
                                var partNumberCode = part.Key;
                                var designQty = part.Value;
                                sql = $@"select c.CatalogId,d.name CatalogName,a.* from PartNumber a  inner join CommodityCode b on b.Id=a.CommodityCodeId  inner join ComponentType c on c.Id=a.ComponentTypeId  inner join Catalog d on d.Id=c.CatalogId  where a.Status=0 and b.Status=0 and c.Status=0 and d.Status=0 and a.code=@partNumberCode";
                                var typeList = Db.Ado.SqlQuery<PartNumberDto>(sql, new { partNumberCode = partNumberCode });
                                if (typeList != null && typeList.Count > 0)
                                {
                                    csv.LogMsg += $"#【SQL】{sql.Replace("@partNumberCode", $"'{ partNumberCode}'")}";
                                    csv.LogMsg += $"#【4】找到 PartNumberDto 数量：{typeList.Count}，partNumberCode={partNumberCode}";
                                    foreach (var type in typeList)
                                    {
                                        var ent = mapList.FirstOrDefault(c => c.CatalogId == type.CatalogId);
                                        if (ent != null)
                                        {
                                            var tempSql = $@"select * from MaterialTakeOffDetail a where a.status=0 and a.PartNumberId='{type.Id}' and a.DeviceId='{deviceId}' and a.ProjectId='{projectId}' and a.CommodityCodeId='{type.CommodityCodeId}' and a.CreateUserId='{ csv.UserId}'";
                                            csv.LogMsg += $"#明细【SQL】{tempSql}";
                                            var detail = await Db.Queryable<MaterialTakeOffDetail>().FirstAsync(c => c.Status == 0 &&c.PartNumberId==type.Id && c.ProjectId == projectId && c.DeviceId == deviceId && c.CommodityCodeId == type.CommodityCodeId && c.CreateUserId == csv.UserId);
                                            if (detail == null)
                                            {
                                                var newDetail = newList.FirstOrDefault(c => c.Status == 0 && c.PartNumberId == type.Id && c.ProjectId == projectId && c.DeviceId == deviceId && c.CommodityCodeId == type.CommodityCodeId && c.CreateUserId == csv.UserId);
                                                if (newDetail == null)
                                                {
                                                    csv.LogMsg += $"#【5】新增 MaterialTakeOffDetail ProjectId={projectId}，deviceId={deviceId}，CommodityCodeId={ type.CommodityCodeId}，CreateUserId={csv.UserId}";
                                                    #region 物资汇总明细表
                                                    detail = new MaterialTakeOffDetail();
                                                    detail.Id = Guid.NewGuid().ToString();
                                                    detail.MaterialTakeOffId = mto.Id;
                                                    detail.CommodityCodeId = type.CommodityCodeId;
                                                    detail.PartNumberId = type.Id;//采购码Id                                                                
                                                    detail.ProjectId = projectId;//所属项目
                                                    detail.DeviceId = deviceId;//所属装置
                                                    detail.DesignQty = designQty;//数量
                                                    detail.Flag = 0;
                                                    detail.Status = 0;
                                                    detail.CreateUserId = csv.UserId;
                                                    detail.CreateTime = DateTime.Now;
                                                    detail.LastModifyUserId = csv.UserId;
                                                    detail.LastModifyTime = DateTime.Now;
                                                    newList.Add(detail);
                                                    // Db.Insertable(detail).ExecuteCommand();
                                                    #endregion                                                   
                                                }
                                                else
                                                {
                                                    newDetail.DesignQty += designQty;//数量
                                                }
                                                csv.Success = true;
                                            }
                                            else
                                            {
                                                csv.LogMsg += $"#【6】更新 MaterialTakeOffDetail DesignQty={detail.DesignQty}改为{designQty}， ProjectId={projectId}，deviceId={deviceId}，CommodityCodeId={ type.CommodityCodeId}，CreateUserId={csv.UserId}";
                                                detail.DesignQty = designQty;//数量
                                                detail.LastModifyUserId = csv.UserId;
                                                detail.LastModifyTime = DateTime.Now;
                                                editList.Add(detail);
                                                // Db.Updateable(detail).ExecuteCommand();
                                                csv.Success = true;
                                            }
                                        }
                                        else
                                        {
                                            csv.Success = false;
                                            csv.ErrorMsg += $"# 找不到 ProjectCatalogMap 的映射关系 {projectObj.Name}与 {type.CatalogName}";
                                            csv.LogMsg += $"#【7】找不到 ProjectCatalogMap 的映射关系 {projectObj.Name}与 {type.CatalogName};projectId={projectId}， CatalogId={ type.CatalogId}";
                                        }
                                    }
                                }
                                else
                                {
                                    csv.Success = false;
                                    csv.ErrorMsg += $"# 找不到 采购码 partNumberCode={partNumberCode}";
                                    csv.LogMsg += $"#【8】找不到 采购码 partNumberCode={partNumberCode}";
                                }
                            }
                        }
                        else
                        {
                            csv.Success = false;
                            csv.ErrorMsg += $"# CSV文件中没有找到 PartNumber（采购码） 和 数量";
                            csv.LogMsg += $"#【2】CSV文件中没有找到 PartNumber 和 数量";
                        }
                    }
                    else
                    {
                        csv.Success = false;
                        csv.ErrorMsg += $"# 找不到项目：{projectObj.Name} 与 Catalog 的关联";
                        csv.LogMsg += $"#【9】找不到项目：{projectObj.Name}与 Catalog 的关联，ProjectId=[{csv.ProjectId} 与 CatalogId=[{string.Join(',', mapList.Select(c => c.CatalogId))}] 关联 ";
                    }
                }
                var num = model.CSVList.Count(c=>c.Success==false);//是否全部可以导入,如果没有false可以导入
                model.Success = num>0?false:true;
                if (num==0)
                {
                    if (newList.Count > 0)
                    {
                      var n= await Db.Insertable(newList).ExecuteCommandAsync();
                      model.LogMsg += $"#【10】新增 MaterialTakeOffDetail 数量：{n}";
                    }
                    if (editList.Count > 0)
                    {
                      var n=  await Db.Updateable(editList).ExecuteCommandAsync();
                      model.LogMsg += $"#【10】更新 MaterialTakeOffDetail 数量：{n}";
                    }
                }
                return model;
            }
            catch (Exception e)
            {
                model.LogMsg+=e.ToString();
                return model;
            }
        }
    }
}