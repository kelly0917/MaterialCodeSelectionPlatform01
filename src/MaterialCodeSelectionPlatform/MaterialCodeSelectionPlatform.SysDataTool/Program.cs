using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace CommodityCodeSelectionPlatform.SysDataTool
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SysDataScheduler>(s =>
                {
                    s.ConstructUsing(name => new SysDataScheduler());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.SetDescription("UDP转发服务");
                x.SetDisplayName("UDP转发服务");
                x.SetServiceName("UDP转发服务");
            });
            Console.Read();

        }
    }
}
