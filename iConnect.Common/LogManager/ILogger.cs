using System;

namespace iConnect.Common
{
    /// <summary>
    /// Logger for logging messages.
    /// </summary>
    public interface ILogger
    {
        string LoggerGuid { get; set; }

        /// <summary>
        /// Logs the specified message as Debug, formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage"></param>
        /// <param name="args">The parameters to format the message with.</param>
        void Debug(string logMessage, params object[] args);

        /// <summary>
        /// Logs the specified message as Info, formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void Info(string logMessage, params object[] args);

        /// <summary>
        /// Logs the specified message as Error, formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void Error(string logMessage, params object[] args);

        /// <summary>
        /// Logs the specified message as Trace, formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void Trace(string logMessage, params object[] args);

        /// <summary>
        /// Logs the specified message as Warn, formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void Warn(string logMessage, params object[] args);

        /// <summary>
        /// Logs the specified message as Fatal, formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void Fatal(string logMessage, params object[] args);
        
        /// <summary>
        /// Logs the specified message as Debug and includes exception information.
        /// The message is formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void DebugException(string logMessage, Exception exception, params object[] args);

        /// <summary>
        /// Logs the specified message as Info and includes exception information.
        /// The message is formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void InfoException(string logMessage, Exception exception, params object[] args);

        /// <summary>
        /// Logs the specified message as Error and includes exception information.
        /// The message is formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void ErrorException(string logMessage, Exception exception, params object[] args);

        /// <summary>
        /// Logs the specified message as Trace and includes exception information.
        /// The message is formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void TraceException(string logMessage, Exception exception, params object[] args);

        /// <summary>
        /// Logs the specified message as Warn and includes exception information.
        /// The message is formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void WarnException(string logMessage, Exception exception, params object[] args);

        /// <summary>
        /// Logs the specified message as Fatal and includes exception information.
        /// The message is formatted with String.Format(message, args) if a log entry is made.
        /// </summary>
        /// <param name="logMessage">The message to log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The parameters to format the message with.</param>
        void FatalException(string logMessage, Exception exception, params object[] args);
    }
}
