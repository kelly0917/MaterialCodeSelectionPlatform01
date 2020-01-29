using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using CommodityCodeSelectionPlatform.SysDataTool.IServices;
using Quartz;

namespace CommodityCodeSelectionPlatform.SysDataTool
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

        private ISysDataService sysDataService;
        public SysDataJob()
        {
            


        }

        public Task Execute(IJobExecutionContext context)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine(DateTime.Now);
            });

            log.Debug($"任务执行耗时：{stopwatch.ElapsedMilliseconds}mm");
        }




    }
}