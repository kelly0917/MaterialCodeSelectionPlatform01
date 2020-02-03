using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MaterialCodeSelectionPlatform.SysDataTool.IServices;
using MaterialCodeSelectionPlatform.SysDataTool.Utilities;

namespace MaterialCodeSelectionPlatform.SysDataTool.Services
{
    public class SysCommandService
    {
        private ILog log = LogManager.GetLogger(typeof(SysCommandService));
        /// <summary>
        /// 开始接受指令
        /// </summary>
        public void StartReceviceCommand()
        {
            Task.Run(() => { addHttpListen();});
        }

        private static object lockObj = new object();

        private void addHttpListen()
        {
            try
            {
                //初始http
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(ConfigurationManager.AppSettings["BindServer"]);
                listener.Start();
                Console.WriteLine("同步指令服务启动成功！");
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
            catch (Exception e)
            {
                log.Error(e);
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