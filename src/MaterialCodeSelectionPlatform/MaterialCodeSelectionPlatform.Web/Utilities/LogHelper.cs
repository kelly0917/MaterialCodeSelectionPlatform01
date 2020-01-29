using System;
using log4net;

namespace CommodityCodeSelectionPlatform.Web
{
    public class LogHelper
    {
        public static ILog GetLogger<T>()
        {
            return LogManager.GetLogger(Startup.repository.Name, typeof(T));
        }
        
    }
}