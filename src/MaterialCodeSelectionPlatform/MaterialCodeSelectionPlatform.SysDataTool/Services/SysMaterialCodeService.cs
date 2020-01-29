using System;
using System.Data;
using System.Linq;
using Common.Logging;
using CommodityCodeSelectionPlatform.SysDataTool.IServices;
using CommodityCodeSelectionPlatform.SysDataTool.Model;
using CommodityCodeSelectionPlatform.SysDataTool.Utilities;

namespace CommodityCodeSelectionPlatform.SysDataTool.Services
{
    public class SysCommodityCodeService: ISysDataService
    {
        private ILog log = LogManager.GetLogger<SysCommodityCodeService>();

        /// <summary>
        /// 同步数据
        /// </summary>
        public void SysData(string catlog)
        {
            if (string.IsNullOrEmpty(catlog))//同步所有编码库
            {
                var catlogs = CacheData.ConfigCache.ToList();

                foreach (var catlog1 in catlogs)
                {
                    realSysData(catlog1);
                }
            }
            else
            {
                realSysData(CacheData.ConfigCache
                    .Where(c => c.CAT_ID.Equals(catlog, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
            }
        }

        private void realSysData(SysConfigModel configModel)
        {

            //物资编码
            var sql = $"SELECT COMMODITY_NO,COMMODITY_ID,COMMODITY_CLASS_NO,CATALOG_NO FROM COMMODITY WHERE CATALOG_NO = {configModel.CAT_ID} AND APPROVAL_STATUS_NO =2  ";
            DataTable table = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName = "Temp_CommodityCode";
            CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName);

            //采购码
            sql = $"select PART_NO,PART_ID,CATLOG_NO,COMMODITY_NO from part where CATALOG_NO = {configModel.CAT_ID} ";
            DataTable table1 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName1 = "Temp_PartNumber";
            CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName1);

            //采购码属性
            sql = $"select CATALOG_NO,INSTANCE_NO,CLASS_NO,ENTITY_PROPERTY_NO,PROPERTY_VALUE from instance_property_value where instance_no = 1580 and entity_property_no = 56";
            DataTable table2 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName2 = "Temp_PartNumberAttribute";
            CommonHelper.SqlBulkCopyInsert(table2, CacheData.SqlConn, tempTableName2);

            //同步字典属性
            sql = $"SELECT ENTITY_PROPERTY_NO,ENTITY_PROPERTY_ID,CATALOG_NO FROM ENTITY_PROPERTY WHERE CATALOG_NO ={configModel.CAT_ID} ";
            DataTable table3 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName3 = "Temp_Property";
            CommonHelper.SqlBulkCopyInsert(table3, CacheData.SqlConn, tempTableName3);


            var SP_Name = "SP_SysCommodityCodeData";
            CommonHelper.ExcuteSP(SP_Name, CacheData.SqlConn);
        }
    }
}