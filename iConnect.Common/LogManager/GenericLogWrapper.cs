using System;


namespace iConnect.Common
{
    /// <summary>
    /// Indicate the level of error being used for logging.
    /// </summary>
    public enum LogErrorLevel
    {
        None = 0,
        Debug = 1,
        Error = 2,
        Fatal = 3,
        Info = 4,       
        Trace = 5,
        Warning = 6
    }

    /// <summary>
    /// Defined to provide generice interface to all loggers.
    /// </summary>
    public abstract class GenericLogWrapper
    {
        /// <summary>
        /// Defined to insert data when a method is being processed.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        public abstract void MethodStart<T>(ILogger logger, LogErrorLevel level, string methodName, T data);

        /// <summary>
        /// Defined to insert data when a method is being processed.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        public abstract void MethodStart(ILogger logger, LogErrorLevel level, string methodName);

        /// <summary>
        /// Defined to log data when a method has finished processing.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        public abstract void MethodEnd<T>(ILogger logger, LogErrorLevel level, string methodName, T data);

        /// <summary>
        /// Defined to log data when a method has finished processing.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="methodName"></param>
        public abstract void MethodEnd(ILogger logger, LogErrorLevel level, string methodName);

        /// <summary>
        /// Defined to log details when an exception is raised.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logException"></param>
        /// <param name="data"></param>
        public abstract void Exception<T>(ILogger logger, LogErrorLevel level, Exception logException, T data);

        /// <summary>
        /// Defined to log details when an exception is raised.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logException"></param>
        public abstract void Exception(ILogger logger, LogErrorLevel level, Exception logException);

        /// <summary>
        /// Defined to log a message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="logMessage"></param>
        public abstract void Message(ILogger logger, LogErrorLevel level, string logMessage);
    }
}

