using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Common.Logging;
using MaterialCodeSelectionPlatform.SysDataTool.IServices;
using MaterialCodeSelectionPlatform.SysDataTool.Model;
using MaterialCodeSelectionPlatform.SysDataTool.Utilities;

namespace MaterialCodeSelectionPlatform.SysDataTool.Services
{
    /// <summary>
    /// 同步元件类型
    /// </summary>
    public class SysClassDataService:ISysDataService
    {
        private ILog log = LogManager.GetLogger<SysClassDataService>();
        
        public SysClassDataService()
        {

        }

        /// <summary>
        /// 同步数据
        /// </summary>
        public void SysData(string catlog=null)
        {
            if (string.IsNullOrEmpty(catlog))//同步所有编码库
            {
               var  catlogs = CacheData.ConfigCache.ToList();

               foreach (var catlog1 in catlogs)
               {
                   realSysData(catlog1);
               }
            }
            else
            {
                realSysData(CacheData.ConfigCache
                    .Where(c => c.Name.Equals(catlog, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
            }
        }

        private void realSysData(SysConfigModel configModel)
        {
            try
            { 
                //and approval_status_no =2
                var sql = $"select  a.CLASS_NO,a.CLASS_ID,a.CATALOG_NO,a.SEQ_NO,a.DESCR,a.PARENT_CLASS_NO,a.CAN_INSTANTIATE,a.UNIT_ID,b.DRAW_DISCIPLINE_ID as DRAW_DISCIPLINE_NO,a.APPROVAL_STATUS_NO  from class a inner join DRAW_DISCIPLINE b on a.DRAW_DISCIPLINE_NO = b.DRAW_DISCIPLINE_NO where a.catalog_no = {configModel.Code}   and a.cat_entity_type_no =3 ";
                DataTable table = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
                var tempTableName = "Temp_ComponentType";

                //递归排除掉 审批状态不为2,

                string deleteSql = $"delete from Temp_ComponentType where CATALOG_NO={configModel.Code}";
                CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);

                CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName);
                var SP_Name = "SP_SysComponentType";

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter()
                {
                    ParameterName = "@UserId",
                    Value = CacheData.AdminUserId,
                    DbType = DbType.String
                });
                parameters.Add(new SqlParameter()
                {
                    ParameterName = "@CatalogNo",
                    Value = configModel.Code,
                    DbType = DbType.Int32
                });
                CommonHelper.ExcuteSP(SP_Name, CacheData.SqlConn, parameters);

            }
            catch (Exception e)
            {
                log.Error(e);

            }
           
        }

    }
}