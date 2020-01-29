using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CommodityCodeSelectionPlatform.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;

            if (File.Exists(path + "appsettings.json"))
            {

            }
            else
            {
                path = Directory.GetCurrentDirectory();
            }
            WebHost.CreateDefaultBuilder()
                .UseKestrel()
                .UseUrls(GetServerUrls(args))
                .UseIISIntegration()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build().Run();
        }

        private static string[] GetServerUrls(string[] args)
        {
            List<string> urls = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if ("--server.urls".Equals(args[i], StringComparison.OrdinalIgnoreCase))
                {
                    urls.Add(args[i + 1]);
                }
            }
            if (urls.Count == 0)
            {
                urls.Add("http://*:5000");
            }
            return urls.ToArray();
        }
    }
}
