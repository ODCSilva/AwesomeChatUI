using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLogger {
    /// <summary>
    /// Omar_Logger class.
    /// </summary>
    public class Omar_Logger : ILoggingService {

        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Writes a line into the console.
        /// </summary>
        /// <param name="message">The string to be appended into the file.</param>
        public void Log(string message) {
            logger.Info(message);
        }
    }
}
