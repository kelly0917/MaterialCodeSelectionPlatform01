using System;
using System.Data;
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
        public void SysData(string catlog)
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
            var sql = $"select  CLASS_NO,CLASS_ID,CATALOG_NO,SEQ_NO,DESCR,PARENT_CLASS_NO from class where catalog_no = {configModel.Code} and approval_status_no =2  and cat_entity_type_no =3 ";
            DataTable table = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName = "Temp_ComponentType";


            CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName);
            var SP_Name = "SP_SysClassData";
            CommonHelper.ExcuteSP(SP_Name, CacheData.SqlConn);
        }

    }
}