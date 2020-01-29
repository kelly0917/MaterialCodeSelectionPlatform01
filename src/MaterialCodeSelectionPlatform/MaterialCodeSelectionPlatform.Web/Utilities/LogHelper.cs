using System;
using log4net;

namespace MaterialCodeSelectionPlatform.Web
{
    public class LogHelper
    {
        public static ILog GetLogger<T>()
        {
            return LogManager.GetLogger(Startup.repository.Name, typeof(T));
        }
        
    }
}