using System;
using System.Globalization;
using NLog;

namespace iConnect.Common
{
    /// <summary>
    /// Implements the ILogger, with a NLog logger as the implementation.
    /// Wraps a NLog logger.
    /// </summary>
    public class NLogLogger : ILogger
    {
        private Logger logger_;
        /// <summary>
        /// NLog logger implementation.
        /// </summary>
        public Logger Logger
        {
            get { return logger_; }
            private set { logger_ = value; }
        }

        private DateTime instanceDateTime_;
        /// <summary>
        /// Instance DateTime
        /// </summary>
        public DateTime InstanceDateTime
        {
            get { return instanceDateTime_; }
            set { instanceDateTime_ = value; }
        }

        private string loggerGuid_;
        /// <summary>
        /// Instance DateTime
        /// </summary>
        public string LoggerGuid
        {
            get { return loggerGuid_; }
            set { loggerGuid_ = value; }
        }

        /// <summary>
        /// Initializes with the specified NLog.Logger implementation.
        /// </summary>
        /// <param name="logger"></param>
        public NLogLogger(Logger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Creates a LogEventInfo object with all information required for the NLog logger to perfom
        /// the logging, including additional Workflow / Activity information.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="logMessage">The message.</param>
        /// <param name="exception">The exception. If null, the exception will be ignored.</param>
        /// <returns>LogEventInfo object.</returns>
        protected virtual LogEventInfo CreateEventInfo(LogLevel logLevel, string logMessage, Exception exception)
        {
            var logEvent = new LogEventInfo(logLevel, Logger.Name, logMessage);

            logEvent.Exception = exception;
            logEvent.Context["InstanceDateTime"] = InstanceDateTime;
            logEvent.Context["LoggerGuid"] = loggerGuid_;

            return logEvent;
        }

        /// <summary>
        /// Creates a LogEventInfo object with all information required for the NLog logger to perform
        /// the logging, including additional Workflow / Activity information.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        protected virtual LogEventInfo CreateEventInfo(LogLevel logLevel, string logMessage)
        {
            return CreateEventInfo(logLevel, logMessage, null);
        }

        /// <summary>
        /// Creates a LogEventInfo object with all information required for the NLog logger to perform
        /// the logging, including additional Workflow / Activity information. No exception information
        /// will be included.
        /// </summary>
        /// <param name="logMessage">The message.</param>        
        /// <returns>LogEventInfo object.</returns>
        protected virtual string FormatMessage(string logMessage, object[] args)
        {
            String sFormattedMsg = logMessage;
            try
            {
                sFormattedMsg = String.Format(CultureInfo.CurrentCulture, logMessage, args);
            }
            catch (FormatException) { }

            return sFormattedMsg;
        }

        #region Implementation of ILogger

        public virtual void Debug(string logMessage, params object[] args)
        {
            if (Logger.IsDebugEnabled)
            {                
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Debug, FormatMessage(logMessage, args)));
            }
        }

        public virtual void Info(string logMessage, params object[] args)
        {
            if (Logger.IsInfoEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Info, FormatMessage(logMessage, args)));
            }
        }

        public virtual void Error(string logMessage, params object[] args)
        {
            if (Logger.IsErrorEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Error, FormatMessage(logMessage, args)));
            }
        }

        public virtual void Trace(string logMessage, params object[] args)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Trace, FormatMessage(logMessage, args)));
            }
        }

        public virtual void Warn(string logMessage, params object[] args)
        {
            if (Logger.IsWarnEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Warn, FormatMessage(logMessage, args)));
            }
        }

        public virtual void Fatal(string logMessage, params object[] args)
        {
            if (Logger.IsFatalEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Fatal, FormatMessage(logMessage, args)));
            }
        }

        public virtual void DebugException(string logMessage, Exception exception, params object[] args)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Debug, FormatMessage(logMessage, args), exception));
            }
        }

        public virtual void InfoException(string logMessage, Exception exception, params object[] args)
        {
            if (Logger.IsInfoEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Info, FormatMessage(logMessage, args), exception));
            }
        }

        public virtual void ErrorException(string logMessage, Exception exception, params object[] args)
        {
            if (Logger.IsErrorEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Error, FormatMessage(logMessage, args), exception));
            }
        }

        public virtual void TraceException(string logMessage, Exception exception, params object[] args)
        {
            if (Logger.IsTraceEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Trace, FormatMessage(logMessage, args), exception));
            }
        }

        public virtual void WarnException(string logMessage, Exception exception, params object[] args)
        {
            if (Logger.IsWarnEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Warn, FormatMessage(logMessage, args), exception));
            }
        }

        public virtual void FatalException(string logMessage, Exception exception, params object[] args)
        {
            if (Logger.IsFatalEnabled)
            {
                Logger.Log(typeof(NLogLogger), CreateEventInfo(LogLevel.Fatal, FormatMessage(logMessage, args), exception));
            }
        }

        #endregion
    }
}
