using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                log.Debug($"开始同步物资编码类别！编码库为:{configModel.Name}");
                //and approval_status_no =2
                var sql = $"select  a.CLASS_NO,a.CLASS_ID,a.CATALOG_NO,a.SEQ_NO,a.DESCR,a.PARENT_CLASS_NO,a.CAN_INSTANTIATE,a.UNIT_ID,b.DRAW_DISCIPLINE_ID as DRAW_DISCIPLINE_NO,a.APPROVAL_STATUS_NO,c.CATALOG_ID  from class a inner join DRAW_DISCIPLINE b on a.DRAW_DISCIPLINE_NO = b.DRAW_DISCIPLINE_NO inner join CATALOG c on a.CATALOG_NO = c.CATALOG_NO where a.catalog_no = {configModel.Code}   and a.cat_entity_type_no = 3 ";
                DataTable table = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
                var tempTableName = "Temp_ComponentType";
                log.Debug($"从Oracle获取数据完成，返回数据量为：{table.Rows.Count},耗时：{stopwatch.ElapsedMilliseconds}mm");
                stopwatch.Restart();
                //递归排除掉 审批状态不为2,
                CacheData.SetDealProgress(1);
                string deleteSql = $"delete from Temp_ComponentType";
                CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);
              
                CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName);
                log.Debug($"批量插入临时表完成,耗时：{stopwatch.ElapsedMilliseconds}mm");
                stopwatch.Restart();
                var SP_Name = "SP_SysComponentType";
                CacheData.SetDealProgress(2);
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter()
                {
                    ParameterName = "@UserId",
                    Value = CacheData.AdminUserId,
                    DbType = DbType.String
                });
                parameters.Add(new SqlParameter()
                {
                    ParameterName = "@CatalogName",
                    Value = configModel.Name,
                    DbType = DbType.String
                });
                CommonHelper.ExcuteSP(SP_Name, CacheData.SqlConn, parameters);
                log.Debug($"同步数据完成，存储过程执行耗时：{stopwatch.ElapsedMilliseconds}mm");
                stopwatch.Stop();
                CacheData.SetDealProgress(3);
            }
            catch (Exception e)
            {
                log.Error("同步物资编码类别报错",e);
            }
           
        }

    }
}