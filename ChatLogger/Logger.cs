namespace ChatLogger {

    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// A class responsible for writing lines into a log file.
    /// </summary>
    public class Logger : ILoggingService {

        /// <summary>
        /// Specifies the name of the directory to save the chat logs into.
        /// </summary>
        private const string LOGS_DIR = "\\logs";

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public Logger() {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            var localPath = new Uri(assemblyPath).LocalPath;
            FilePath = localPath + LOGS_DIR;
            FileName = "\\chat-" + DateTime.Now.ToString("dd-MM-yyyy-HHmm") + ".log";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the file that the log is being written to.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets or sets the path in which the log file will reside.
        /// </summary>
        public string FilePath { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Saves a line into the log file.
        /// </summary>
        /// <param name="line">The string to be appended into the file.</param>
        public void Log(string line) {
            if (!Directory.Exists(FilePath)) {
                DirectoryInfo di = Directory.CreateDirectory(FilePath);
            }

            var timeStamp = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt - ");

            using (var writer = new StreamWriter(FilePath + FileName, true, Encoding.UTF8)) {
                writer.WriteLineAsync(timeStamp + line);
                writer.Close();
            }
        }

        #endregion Methods
    }
}