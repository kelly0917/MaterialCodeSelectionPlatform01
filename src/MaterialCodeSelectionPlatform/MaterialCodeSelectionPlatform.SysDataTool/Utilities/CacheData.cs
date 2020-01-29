using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using MaterialCodeSelectionPlatform.SysDataTool.Model;
using Newtonsoft.Json;

namespace MaterialCodeSelectionPlatform.SysDataTool.Utilities
{
    public static class CacheData
    {
        public static List<SysConfigModel> ConfigCache { get; set; }

        public static string AdminUserId { get; set; }

        public static string SqlConn { get; set; }

        static CacheData()
        {
            SqlConn = ConfigurationManager.AppSettings["SqlConn"];
            AdminUserId = ConfigurationManager.AppSettings["AdminUserId"];
            string sql = "select * from Catalog";
            ConfigCache = CommonHelper.GetDataFromSqlServer<SysConfigModel>(sql,SqlConn);

            ////string sss =
            ////    "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.3.12)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=VM-DEMO103)));Persist Security Info=True;User ID =ERM; Password=lmc;";
            // string sss = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.3.12)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=ERMVM11)));User Id=DEMO103; Password=DEMO103";
            // //(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = ERMVM11)))
            //var sql1 = $"SELECT COMMODITY_NO,COMMODITY_ID,COMMODITY_CLASS_NO,CATALOG_NO FROM COMMODITY WHERE CATALOG_NO = 41 AND APPROVAL_STATUS_NO =2  ";
            //DataTable table = CommonHelper.GetDataFromOracle(sql1, sss);

        }


    }

}