using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using MaterialCodeSelectionPlatform.SysDataTool.IServices;
using MaterialCodeSelectionPlatform.SysDataTool.Services;
using MaterialCodeSelectionPlatform.SysDataTool.Utilities;
using Quartz;
using Quartz.Impl;

namespace MaterialCodeSelectionPlatform.SysDataTool
{
    /// <summary>
    /// 调度器，控制任务的执行
    /// </summary>
    public class SysDataScheduler
    {
        private ILog log = LogManager.GetLogger<SysDataScheduler>();
        /// <summary>
        /// 启动job
        /// </summary>
        public async void Start()
        {
            try
            {
                SysCommandService sysCommandService = new SysCommandService();
                sysCommandService.StartReceviceCommand();


                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                IScheduler scheduler = await schedulerFactory.GetScheduler();
                await scheduler.Start();

                var time = 5.0;
                var configTime = ConfigurationManager.AppSettings["jobTime"];
                if (double.TryParse(configTime, out time))
                {
                    time = time * 60;
                }
                IJobDetail job = JobBuilder.Create<SysDataJob>().WithIdentity("SysDataJob", "groupa").Build();

                ITrigger trigger = TriggerBuilder.Create().WithIdentity("sysdatatrigger", "groupa")
                      //.WithSimpleSchedule(b => b.WithIntervalInSeconds((int)time).RepeatForever())
                      .WithCronSchedule(ConfigurationManager.AppSettings["jobExp"]).StartNow()
                    .Build();
                scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception e)
            {
                log.Error("任务调度启动失败", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public async void Stop()
        {
            log.Debug("任务调度退出");
        }

       
    }
}