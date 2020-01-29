using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using CommodityCodeSelectionPlatform.Data.Exentions.DependencyInjection;
using CommodityCodeSelectionPlatform.Service.Exentions.DependencyInjection;
using CommodityCodeSelectionPlatform.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommodityCodeSelectionPlatform.Web
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            repository = LogManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));


            Config.DBProviderName = Configuration["connectStrings:ProviderName"];
            Config.DBConnectionString = Configuration["connectStrings:ConnectionString"];
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = new PathString("/Login/Index");
                    o.AccessDeniedPath = new PathString("/Error/Forbidden");
                });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            // 添加Dao层服务
            services.AddScopedDao();
            // 添加Service层服务
            services.AddScopedService();
            // 添加发现客户端服务
            //services.AddDiscoveryClient(Configuration);
            // 添加日志服务
            services.AddLogging();
            services.AddMvc(optons =>
            {
                optons.Filters.Add<ExceptionFilter>();
            });
            //允许跨域访问
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddDistributedMemoryCache();
            services.AddSession();
         



        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
           
            loggerFactory.AddConsole(LogLevel.Debug);

            
            // 开发配置中间件
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseCors("default");
            app.UseAuthentication();
            var log = loggerFactory.CreateLogger(this.GetType());
         

            app.UseSession();
            app.UseMvcWithDefaultRoute();
            app.UseHttpsRedirection();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }



    }
}
