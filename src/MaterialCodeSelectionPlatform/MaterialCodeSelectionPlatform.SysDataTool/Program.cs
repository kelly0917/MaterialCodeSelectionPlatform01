using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace MaterialCodeSelectionPlatform.SysDataTool
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
                x.SetDescription("材料编码同步服务");
                x.SetDisplayName("材料编码同步服务");
                x.SetServiceName("材料编码同步服务");
            });
            Console.Read();

        }
    }
}
