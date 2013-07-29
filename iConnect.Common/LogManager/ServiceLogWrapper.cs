using System;
using System.Globalization;


namespace iConnect.Common
{
    /// <summary>
    /// Defined class to log RMAService specific data.
    /// </summary>
    public class ServiceLogWrapper : GenericLogWrapper
    {
        private const string Start = "Start::";
        private const string End = "End::";
        private const string ExceptionText = "Exception::";
        private const string TraceText = "Trace::";
        private const string MessageText = "Message::";
        private static ServiceLogWrapper _serviceLogger;
        /// <summary>
        /// Gets the logger object to log data in a file.
        /// </summary>
        public static ServiceLogWrapper GetServiceLogger
        {
            get
            {
                if (_serviceLogger == null)
                    _serviceLogger = new ServiceLogWrapper();

                return _serviceLogger;
            }
        }


        #region Private Methods
        /// <summary>
        /// Generic method to log data related to a method.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loggerGuid"></param>
        /// <param name="methodName"></param>
        /// <param name="methodPrefix"></param>
        /// <param name="data"></param>
        private static void Method<T>(ILogger logger, LogErrorLevel level, string infoMessage, T data)
        {
            string message = typeof(T) != typeof(System.IO.Stream) ? (typeof(T) != typeof(string) ? XmlSerialization.ToXmlString(data) : data.ToString()) : XmlSerialization.ToFormattedString(data as System.IO.Stream);
            string formattedMessage = infoMessage != null ? string.Format(CultureInfo.CurrentCulture, "Method ::{0} || Data::{1}",
                                               infoMessage,
                                               message) : message;

            LogMessage(logger, level, formattedMessage);

        }

        /// <summary>
        /// Generic method to log message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        private static void LogMessage(ILogger logger, LogErrorLevel level, string message)
        {
            switch (level)
            {
                case LogErrorLevel.Debug:
                    logger.Debug(message);
                    break;
                case LogErrorLevel.Error:
                    logger.Error(message);
                    break;
                case LogErrorLevel.Fatal:
                    logger.Fatal(message);
                    break;
                case LogErrorLevel.Info:
                    logger.Info(message);
                    break;
                case LogErrorLevel.Trace:
                    logger.Trace(message);
                    break;
                case LogErrorLevel.Warning:
                    logger.Warn(message);
                    break;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Defined to log data when a method is being processed.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        /// <param name="logger"></param>
        /// <param name="data"></param>
        public override void MethodStart<T>(ILogger logger, LogErrorLevel level, string methodName, T data)
        {
            string formattedMessage = methodName != null ? string.Format(CultureInfo.CurrentCulture, Start + "{0}",
                methodName) : null;
            Method(logger, level, formattedMessage, data);
        }

        /// <summary>
        /// Defined to insert data when a method is being processed.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        /// <param name="logger"></param>
        public override void MethodStart(ILogger logger, LogErrorLevel level, string methodName)
        {
            string formattedMessage = string.Format(CultureInfo.CurrentCulture, Start + "{0}",
                                       methodName);

            LogMessage(logger, level, formattedMessage);
        }


        /// <summary>
        /// Defined to log data when a method has finished processing.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        /// <param name="logger"></param>
        /// <param name="data"></param>
        public override void MethodEnd<T>(ILogger logger, LogErrorLevel level, string methodName, T data)
        {
            string formattedMessage = methodName != null ? string.Format(CultureInfo.CurrentCulture, End + "{0}",
                methodName) : null;
            Method(logger, level, formattedMessage, data);
        }

        /// <summary>
        /// Defined to log data when a method has finished processing.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        /// <param name="logger"></param>
        public override void MethodEnd(ILogger logger, LogErrorLevel level, string methodName)
        {
            string formattedMessage = string.Format(CultureInfo.CurrentCulture, End + "{0}",
                                           methodName);
            LogMessage(logger, level, formattedMessage);
        }


        /// <summary>
        /// Defined to log details when an exception is raised.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="exception"></param>
        /// <param name="data"></param>
        public override void Exception<T>(ILogger logger, LogErrorLevel level, Exception exception, T data)
        {
            if (exception == null)
            {
                return;
            }
            string formattedMessage = string.Format(CultureInfo.CurrentCulture, ExceptionText + "{0} || " + TraceText + "{1} || Data::{2}",
                                           exception.Message,
                                           exception.StackTrace, typeof(T) != typeof(System.IO.Stream) ? XmlSerialization.ToXmlString(data) : XmlSerialization.ToFormattedString(data as System.IO.Stream));
            LogMessage(logger, level, formattedMessage);
        }

        /// <summary>
        /// Defined to log details when an exception is raised.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logException"></param>
        public override void Exception(ILogger logger, LogErrorLevel level, Exception logException)
        {
            if(logException == null)
            {
                return;
            }
            string formattedMessage = string.Format(CultureInfo.CurrentCulture, ExceptionText +"{0}",
                                           logException.Message);
            LogMessage(logger, level, formattedMessage);
        }

        /// <summary>
        /// Defined to log a message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logMessage"></param>
        public override void Message(ILogger logger, LogErrorLevel level, string logMessage)
        {
            string formattedMessage = string.Format(CultureInfo.CurrentCulture, MessageText + "{0}", logMessage);
            LogMessage(logger, level, formattedMessage);
        }

        #endregion
    }
}
