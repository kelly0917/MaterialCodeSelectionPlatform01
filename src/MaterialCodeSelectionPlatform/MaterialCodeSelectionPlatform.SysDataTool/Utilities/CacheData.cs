using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using MaterialCodeSelectionPlatform.SysDataTool.Model;
using Newtonsoft.Json;

namespace MaterialCodeSelectionPlatform.SysDataTool.Utilities
{
    public static class CacheData
    {
        public static List<SysConfigModel> ConfigCache { get; set; }

        public static string SqlConn { get; set; }


        static CacheData()
        {
            SqlConn = ConfigurationManager.AppSettings["SqlConn"];

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "SysConfig.json");
            var configStr = File.ReadAllText(configPath);
            ConfigCache = JsonConvert.DeserializeObject<List<SysConfigModel>>(configPath);
        }


    }
}