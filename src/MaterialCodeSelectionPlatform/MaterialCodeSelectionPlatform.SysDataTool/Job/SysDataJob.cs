using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using MaterialCodeSelectionPlatform.SysDataTool.IServices;
using MaterialCodeSelectionPlatform.SysDataTool.Services;
using MaterialCodeSelectionPlatform.SysDataTool.Utilities;
using Quartz;

namespace MaterialCodeSelectionPlatform.SysDataTool
{
    /// <summary>
    /// 特性
    /// </summary>
    [DisallowConcurrentExecution]//等待上一个执行的时间
    public class SysDataJob : IJob
    {
        /// <summary>
        /// 日志
        /// </summary>
        private ILog log = LogManager.GetLogger<SysDataJob>();

        private ISysDataService sysClassDataService;
        private ISysDataService sysMaterialCodeService;
        public SysDataJob()
        {
            sysClassDataService = new SysClassDataService();
            sysMaterialCodeService = new SysCommodityCodeService();
        }

        public Task Execute(IJobExecutionContext context)
        {
            log.Debug("开始执行任务");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (CacheData.DicCacheSysData.Keys.Count > 0)
            {
                foreach (var key in CacheData.DicCacheSysData.Keys)
                {
                    log.Debug($"存在正在同步中操作，操作Id={key},开始时间为：{CacheData.DicCacheSysData[key]}，总数量为：{CacheData.DicCacheSysData.Keys.Count}");
                }
            }
            else
            {
                CacheData.DicCacheSysData.Add(Guid.Empty.ToString(),DateTime.Now);
                CacheData.CurrentDealKey = Guid.Empty.ToString();
                CacheData.DicDealProgressRate.Add(Guid.Empty.ToString(), 0);

                var catlogs = CacheData.ConfigCache.ToList();
                Console.WriteLine("开始同步数据");
                foreach (var catlog1 in catlogs)
                {
                    Stopwatch stopwatch1 = new Stopwatch();
                    stopwatch1.Start();
                    sysClassDataService.SysData(catlog1.Name);
                    sysMaterialCodeService.SysData(catlog1.Name);
                    log.Debug($"编码库{catlog1.Name}同步完成，耗时：{stopwatch1.ElapsedMilliseconds}mm");
                    stopwatch1.Stop();
                    Console.WriteLine($"编码库{catlog1.Name}同步完成，耗时：{stopwatch1.ElapsedMilliseconds}mm");
                }

                
                
                CacheData.DicCacheSysData.Clear();
                CacheData.DicDealProgressRate.Clear();
                log.Debug($"任务执行耗时：{stopwatch.ElapsedMilliseconds}mm");
                stopwatch.Stop();
            }
            return null;
        }




    }
}