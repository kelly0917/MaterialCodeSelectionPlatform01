using System;
using System.Configuration;
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
                Task.Factory.StartNew(addHttpListen, TaskCreationOptions.LongRunning);

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

                ITrigger trigger = TriggerBuilder.Create().WithIdentity("sysdatatrigger", "groupa").StartNow()
                    .WithSimpleSchedule(b => b.WithIntervalInSeconds((int)time).RepeatForever())
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


        private void addHttpListen()
        {
            //初始http
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(ConfigurationManager.AppSettings["BindServer"]);
            listener.Start();
            log.Debug($"开始监听http请求：{ConfigurationManager.AppSettings["BindServer"]}");
            ISysDataService sysClassDataService = new SysClassDataService();
            ISysDataService sysMaterialCodeService = new SysCommodityCodeService();
            //接收数据
            while (true)
            {
                var context = listener.GetContext();
                try
                {
                    //针对get请求直接返回数据
                    //请求demo http://127.0.0.1:4001/?sysId=123123&opType=start
                    if (context.Request.HttpMethod.ToLower() == "get")
                    {
                        var sysId = context.Request.QueryString["sysId"].ToLower().Trim();
                        var opType = context.Request.QueryString["opType"];

                        if (opType == "start")
                        {
                            if (CacheData.DicCacheSysData.Keys.Count > 0)
                            {
                                foreach (var key in CacheData.DicCacheSysData.Keys)
                                {
                                    log.Debug($"存在正在同步中操作，操作Id={key},开始时间为：{CacheData.DicCacheSysData[key]}，总数量为：{CacheData.DicCacheSysData.Keys.Count}");
                                    returnHttp(context, "success");
                                }
                            }
                            else
                            {
                                CacheData.DicCacheSysData.Add(sysId, DateTime.Now);
                                CacheData.CurrentDealKey = sysId;
                                CacheData.DicDealProgressRate.Add(sysId, 0);
                                Task.Run(() =>
                                {
                                    sysClassDataService.SysData();
                                    sysMaterialCodeService.SysData();

                                    CacheData.DicCacheSysData.Clear();
                                    CacheData.DicDealProgressRate.Clear();
                                });
                                returnHttp(context, "success");
                            }
                        }
                        else
                        {
                            returnHttp(context, CacheData.GetDealProgress());
                        }


                    }
                }
                catch (Exception ex)
                {
                    log.Error("http指令同步数据出错：", ex);
                }
            }



        }
        private void returnHttp(HttpListenerContext context, string content)
        {
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;
            var str = content;
            var bufferWrite = Encoding.Default.GetBytes(str);
            context.Response.OutputStream.Write(bufferWrite, 0, bufferWrite.Length);
            context.Response.Close();
        }

    }
}