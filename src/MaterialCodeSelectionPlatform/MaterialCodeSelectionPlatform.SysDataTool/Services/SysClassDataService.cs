using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
            var sql = $"select  CLASS_NO,CLASS_ID,CATALOG_NO,SEQ_NO,DESCR,PARENT_CLASS_NO,CAN_INSTANTIATE,UNIT_ID,DRAW_DISCIPLINE_NO from class where catalog_no = {configModel.Code} and approval_status_no =2  and cat_entity_type_no =3 ";
            DataTable table = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName = "Temp_ComponentType";

            string deleteSql = $"delete from Temp_ComponentType where CATALOG_NO={configModel.Code}";
            CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);

            CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName);
            var SP_Name = "SP_SysClassData";

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
            CommonHelper.ExcuteSP(SP_Name, CacheData.SqlConn);
        }

    }
}