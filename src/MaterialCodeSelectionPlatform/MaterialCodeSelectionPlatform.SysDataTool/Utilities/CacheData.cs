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

        /// <summary>
        /// 当前处理的Key  DicCacheSysData和DicDealProgressRate 的key
        /// </summary>
        public static string CurrentDealKey { get; set; }

        public static double CurrentProgress { get; set; }
        /// <summary>
        /// 全局同步记录
        /// </summary>
        public static Dictionary<string,DateTime> DicCacheSysData { get; set; } = new Dictionary<string, DateTime>();



        /// <summary>
        /// 同步总共几步
        /// </summary>
        public static int SumDealProgress = 15;

        /// <summary>
        /// 处理进度百分比
        /// </summary>
        public static Dictionary<string,int> DicDealProgressRate { get; set; } = new Dictionary<string, int>();

        public static string AdminUserId { get; set; }

        public static string SqlConn { get; set; }

        static CacheData()
        {
            SqlConn = ConfigurationManager.AppSettings["SqlConn"];
            AdminUserId = ConfigurationManager.AppSettings["AdminUserId"];
            string sql = "select * from Catalog";
            ConfigCache = CommonHelper.GetDataFromSqlServer<SysConfigModel>(sql,SqlConn);
        }

        /// <summary>
        /// 处理到第几步
        /// </summary>
        /// <param name="p"></param>
        public static void SetDealProgress(int p)
        {
            DicDealProgressRate[CacheData.CurrentDealKey] = p;
            CurrentProgress = (DicDealProgressRate[CurrentDealKey] * 100.0 / SumDealProgress);
        }

        /// <summary>
        /// 获取当前处理进度
        /// </summary>
        /// <returns></returns>
        public static string GetDealProgress()
        {
            return CurrentProgress.ToString("#0.00");
        }


    }

}