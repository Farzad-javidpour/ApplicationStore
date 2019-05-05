using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore
{
    public static class MyLogger
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Log(string message)
        {
            logger.Info(message);
        }
    }
}
