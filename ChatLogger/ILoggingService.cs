using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLogger {
    /// <summary>
    /// ILogggingService Interface
    /// </summary>
    public interface ILoggingService {
        /// <summary>
        /// Submits a log
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        void Log(string message);
    }
}
