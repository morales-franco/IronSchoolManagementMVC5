using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronSchool.Utils
{
    public class Services
    {
        private static log4net.ILog _logger;
        /// <summary>
        /// Returns a logger for log operations
        /// </summary>
        /// <example>
        /// Utils.Services.Logger.Debug("Debug logging");
        /// Utils.Services.Logger.Info("Info logging");
        /// Utils.Services.Logger.Warn("Warn logging");
        /// Utils.Services.Logger.Error("Error logging");
        /// Utils.Services.Logger.Fatal("Fatal logging");
        /// </example>
        public static log4net.ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = log4net.LogManager.GetLogger("ApplicationLog");
                }
                return _logger;
            }
        }
    }
}
