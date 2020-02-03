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

        ///// <summary>
        ///// 当前处理的Key  DicCacheSysData和DicDealProgressRate 的key
        ///// </summary>
        //public static string CurrentDealKey { get; set; }

        public static double CurrentProgress { get; set; }

        public static double CurrentIntPro { get; set; } = 0;
        ///// <summary>
        ///// 全局同步记录
        ///// </summary>
        //public static Dictionary<string,DateTime> DicCacheSysData { get; set; } = new Dictionary<string, DateTime>();

        /// <summary>
        /// 是否在进行同步中
        /// </summary>
        public static bool IsSysDataing { get; set; }

        /// <summary>
        /// 单个编码库完成 的步骤
        /// </summary>
        public static int BaseProgress = 15;

        /// <summary>
        /// 同步总共几步
        /// </summary>
        public static int SumDealProgress = 15;

        ///// <summary>
        ///// 处理进度百分比
        ///// </summary>
        //public static Dictionary<string,int> DicDealProgressRate { get; set; } = new Dictionary<string, int>();

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
        public static void SetDealProgress()
        {
            //DicDealProgressRate[CacheData.CurrentDealKey] = p;
            CurrentIntPro = CurrentIntPro + 1;
            Console.WriteLine("CurrentIntPro="+ CurrentIntPro);
            Console.WriteLine("SumDealProgress=" + SumDealProgress);
            CurrentProgress = CurrentIntPro * 100.0 / SumDealProgress;
        }

        /// <summary>
        /// 获取当前处理进度
        /// </summary>
        /// <returns></returns>
        public static string GetDealProgress()
        {
            return CurrentProgress.ToString("#0");
        }


    }

}