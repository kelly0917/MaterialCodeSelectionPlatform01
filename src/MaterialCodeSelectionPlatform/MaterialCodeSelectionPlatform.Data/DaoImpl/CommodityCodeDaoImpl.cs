using MaterialCodeSelectionPlatform.Domain;
using MaterialCodeSelectionPlatform.Domain.Entities;
using MaterialCodeSelectionPlatform.Utilities;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
             ) b ON a.Id = b.CommodityCodeId
            WHERE a.Status = 0 AND a.CommodityCodeId = @CommodityCodeId";
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
        public async Task<List<PartNumberDto>> Save(List<PartNumberDto> list)
        {
            #region sql
            /*
            SELECT * FROM CommodityCodeAttribute WHERE CommodityCodeId='F65F077F-623B-4325-88B1-C39CADACDC7D' AND Status=0
            */
            #endregion
            if (list != null && list.Count > 0)
            {
                
            }
            //var query = Db.Queryable<CommodityCodeAttribute>();
            //if (id.IsNotNullAndNotEmpty())
            //{
            //    query = query.Where(a => a.CommodityCodeId == id && a.Status == 0).OrderBy(a => a.AttributeName);
            //    // 更新点击的次数
            //    var value = Db.Updateable<CommodityCode>().UpdateColumns(it => new { it.Hits }).ReSetValue(it => it.Hits == (it.Hits + 1)).Where(it => it.Id == id).ExecuteCommand();

            //}
            //var list = await query.ToListAsync();
            return list;
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