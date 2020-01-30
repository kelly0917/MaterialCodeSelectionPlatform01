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
    public class SysCommodityCodeService: ISysDataService
    {
        private ILog log = LogManager.GetLogger<SysCommodityCodeService>();

        /// <summary>
        /// 同步数据
        /// </summary>
        public void SysData(string catlog = null)
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
                    .Where(c => c.Name.Equals(catlog, StringComparison.OrdinalIgnoreCase)).FirstOrDefault());
            }
        }

        private void realSysData(SysConfigModel configModel)
        {
            string deleteSql = $"delete from Temp_CommodityCode where CATALOG_NO={configModel.Code}";
            CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);
            //物资编码
            var sql = $"SELECT COMMODITY_NO,COMMODITY_ID,COMMODITY_CLASS_NO,CATALOG_NO FROM COMMODITY WHERE CATALOG_NO = {configModel.Code} AND APPROVAL_STATUS_NO =2  ";
            DataTable table = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName = "Temp_CommodityCode";
            CommonHelper.SqlBulkCopyInsert(table, CacheData.SqlConn, tempTableName);

            //采购码
            deleteSql = $"delete from Temp_PartNumber where CATALOG_NO={configModel.Code}";
            CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);

            sql = $"select PART_NO,PART_ID,CATALOG_NO,COMMODITY_NO,SIZE_REF_NO from part where CATALOG_NO = {configModel.Code} ";
            DataTable table1 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName1 = "Temp_PartNumber";
            CommonHelper.SqlBulkCopyInsert(table1, CacheData.SqlConn, tempTableName1);

            //采购码属性
            deleteSql = $"delete from Temp_PartNumberAttribute where CATALOG_NO={configModel.Code}";
            CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);

            sql = $"select CATALOG_NO,INSTANCE_NO,CLASS_NO,ENTITY_PROPERTY_NO,PROPERTY_VALUE from instance_property_value where CATALOG_NO  = {configModel.Code} ";
            DataTable table2 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName2 = "Temp_PropertyValue";
            CommonHelper.SqlBulkCopyInsert(table2, CacheData.SqlConn, tempTableName2);

            //同步字典属性
            deleteSql = $"delete from Temp_Property where CATALOG_NO={configModel.Code}";
            CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);

            sql = $"SELECT ENTITY_PROPERTY_NO,ENTITY_PROPERTY_ID,CATALOG_NO FROM ENTITY_PROPERTY WHERE CATALOG_NO ={configModel.Code} ";
            DataTable table3 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName3 = "Temp_Property";
            CommonHelper.SqlBulkCopyInsert(table3, CacheData.SqlConn, tempTableName3);


            //同步物资编码属性值
            
            sql =
                string.Format(@"select distinct a.descr, av.VALUE_TEXT ,iav.instance_no,iav.catalog_no,ca.SEQ_NO  from instance_attrib_value iav, attrib_equiv_set aes, attrib a, attrib_value av, attrib_represent ar, 
                    represent_type rt,
                    commodity c,
                    class_attrib ca
                    where  iav.attrib_equiv_set_no = aes.attrib_equiv_set_no
                    and aes.attrib_no = a.attrib_no
                    and aes.ATTRIB_EQUIV_SET_NO = av.ATTRIB_EQUIV_SET_NO
                    and iav.catalog_no = {0}
                    and av.attrib_represent_no  = ar.attrib_represent_no
                    and ar.represent_type_no = rt.represent_type_no
                    and rt.represent_type_id ='{1}'
                    and ca.attrib_no  = a.ATTRIB_NO
                    and ca.CATALOG_NO =  iav.CATALOG_NO
                    and ca.cat_entity_type_no  =1
                    and iav.instance_no =c.COMMODITY_NO
                    and iav.CATALOG_NO = c.CATALOG_NO
                    and c.COMMODITY_CLASS_NO = ca.CLASS_NO
                    order by   ca.SEQ_NO", configModel.Code, configModel.COMM_REPRESENT_TYPE);


            deleteSql = $"delete from Temp_CCPropertyValue where CATALOG_NO={configModel.Code}";
            CommonHelper.ExcuteSql(deleteSql, CacheData.SqlConn);

            DataTable table4 = CommonHelper.GetDataFromOracle(sql, configModel.ConnectionString);
            var tempTableName4 = "Temp_CCPropertyValue";
            CommonHelper.SqlBulkCopyInsert(table4, CacheData.SqlConn, tempTableName4);


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
            CommonHelper.ExcuteSP("SP_SysCommodityCode", CacheData.SqlConn, parameters.ToList());

            parameters = new List<SqlParameter>();
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
            CommonHelper.ExcuteSP("SP_SysPartNumber", CacheData.SqlConn, parameters.ToList());


        }
    }
}