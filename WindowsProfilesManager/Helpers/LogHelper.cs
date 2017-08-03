using System;
using log4net;
using log4net.Appender;

namespace WindowsProfilesManager
{
    public class LogHelper
    {
        private static readonly ILog windowsProfilesManagerLogger = LogManager.GetLogger("WindowsProfilesManager");

        /// <summary>
        /// Writes INFO level message
        /// </summary>
        public static void Info(object message)
        {
            windowsProfilesManagerLogger.Info(message);
        }

        /// <summary>
        /// Writes INFO level message
        /// </summary>
        public static void Info(string format, params object[] args)
        {
            windowsProfilesManagerLogger.InfoFormat(format, args);
        }

        /// <summary>
        /// Writes ERROR level message
        /// </summary>
        public static void Error(object message)
        {
            windowsProfilesManagerLogger.Error(message);
        }

        /// <summary>
        /// Writes ERROR level message
        /// </summary>
        public static void Error(string format, params object[] args)
        {
            windowsProfilesManagerLogger.ErrorFormat(format, args);
        }

        /// <summary>
        /// Writes DEBUG level message
        /// </summary>
        public static void Debug(object message)
        {
            windowsProfilesManagerLogger.Debug(message);
        }

        /// <summary>
        /// Writes DEBUG level message
        /// </summary>
        public static void Debug(string format, params object[] args)
        {
            windowsProfilesManagerLogger.DebugFormat(format, args);
        }

        /// <summary>
        /// Log and print to console
        /// </summary>
        public static void InfoAndPrint(string message, params object[] args)
        {
            Info(message, args);
            Console.WriteLine(message, args);
        }

        /// <summary>
        /// Log and print to console
        /// </summary>
        public static void InfoAndPrint(string message, ConsoleColor color, params object[] args)
        {
            Info(message, args);

            Console.ForegroundColor = color;
            Console.WriteLine(message, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Log and print to console
        /// </summary>
        public static void ErrorAndPrint(object message)
        {
            Error(message);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Log and print to console
        /// </summary>
        public static void ErrorAndPrint(string message, params object[] args)
        {
            Error(message, args);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Set specific log file name
        /// </summary>
        public static void SetCustomFileName(string fileName)
        {
            var h = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();

            foreach (IAppender a in h.Root.Appenders)
            {
                FileAppender fa = (FileAppender)a;

                // Define location and set file name
                string currentLogPath = System.IO.Path.GetDirectoryName(fa.File);
                string newFileName = System.IO.Path.Combine(currentLogPath, fileName);

                fa.File = newFileName;
                fa.ActivateOptions();
            }
        }
    }
}