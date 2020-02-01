using System;
using log4net;

namespace MaterialCodeSelectionPlatform
{
    public class LogHelper
    {
        public static string RepositoryName { get; set; }

        public static ILog GetLogger<T>()
        {
            return LogManager.GetLogger(RepositoryName, typeof(T));
        }
        
    }
}