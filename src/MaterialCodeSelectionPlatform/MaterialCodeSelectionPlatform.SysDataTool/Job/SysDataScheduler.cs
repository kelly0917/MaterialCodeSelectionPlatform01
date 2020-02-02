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

        private static object lockObj = new object();

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
                    lock (lockObj)
                    {
                        //针对get请求直接返回数据
                        //请求demo http://127.0.0.1:4001/?sysId=123123&opType=start
                        if (context.Request.HttpMethod.ToLower() == "get")
                        {
                            //var sysId = context.Request.QueryString["sysId"].ToLower().Trim();
                            var opType = context.Request.QueryString["opType"];

                            Console.WriteLine("接收到URL地址是：" + context.Request.Url);
                            if (opType == "start")
                            {
                                if (CacheData.IsSysDataing)
                                {
                                    //foreach (var key in CacheData.DicCacheSysData.Keys)
                                    //{
                                    //    log.Debug($"存在正在同步中操作，操作Id={key},开始时间为：{CacheData.DicCacheSysData[key]}，总数量为：{CacheData.DicCacheSysData.Keys.Count}");
                                    returnHttp(context, "success");
                                    //}
                                }
                                else
                                {
                                    Console.WriteLine("开始启动同步！");
                                    //CacheData.DicCacheSysData.Add(sysId, DateTime.Now);
                                    //CacheData.CurrentDealKey = sysId;
                                    Task.Run(() =>
                                    {
                                        var catlogs = CacheData.ConfigCache.ToList();
                                        try
                                        {
                                            CacheData.IsSysDataing = true;
                                            CacheData.CurrentIntPro = 0;
                                            CacheData.CurrentProgress = 0;
                                            CacheData.SumDealProgress = CacheData.BaseProgress * catlogs.Count;
                                            foreach (var catlog1 in catlogs)
                                            {
                                                Stopwatch stopwatch1 = new Stopwatch();
                                                stopwatch1.Start();

                                                try
                                                {
                                                    sysClassDataService.SysData(catlog1.Name);
                                                    sysMaterialCodeService.SysData(catlog1.Name);
                                                }
                                                catch (Exception e)
                                                {

                                                    log.Error(e);
                                                }

                                                log.Debug($"编码库{catlog1.Name}同步完成，耗时：{stopwatch1.ElapsedMilliseconds}mm");
                                                stopwatch1.Stop();
                                                Console.WriteLine($"编码库{catlog1.Name}同步完成，耗时：{stopwatch1.ElapsedMilliseconds}mm");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            log.Error(e);
                                        }
                                        finally
                                        {
                                            CacheData.IsSysDataing = false;
                                        }
                                        //CacheData.DicCacheSysData.Clear();
                                        //CacheData.DicDealProgressRate.Clear();

                                        Console.WriteLine("同步完成！");
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