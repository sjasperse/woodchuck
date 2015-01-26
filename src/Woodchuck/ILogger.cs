using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck
{
    /// <summary>
    /// A logging instance.
    /// If implementing a custom logger, please consider inheriting from the Logger base class.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets or sets a list of log levels to be handled by this instance.
        /// For performance reason, ShouldLog takes this into account to filter what is logged.
        /// This same thing can be done in log4net - but this will shortcut all the context building.
        /// </summary>
        IEnumerable<LogLevel> LogLevelsHandled { get; set; }

        /// <summary>
        /// Check if this instance wants to log this log level
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <returns>True if should be logged.</returns>
        bool ShouldLog(LogLevel logLevel);

        /// <summary>
        /// Write an INFO message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties object to be added to the log entry properties (Ex: new { prop1 = "test" }) </param>
        void Info(string message, object otherProps = null);

        /// <summary>
        /// Write an INFO message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties dictionary to be added to the log entry properties</param>
        void Info(string message, Dictionary<string, object> otherProps);

        /// <summary>
        /// Write an DEBUG message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties object to be added to the log entry properties (Ex: new { prop1 = "test" }) </param>
        void Debug(string message, object otherProps = null);

        /// <summary>
        /// Write an DEBUG message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties dictionary to be added to the log entry properties</param>
        void Debug(string message, Dictionary<string, object> otherProps);

        /// <summary>
        /// Write an ERROR message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties object to be added to the log entry properties (Ex: new { prop1 = "test" }) </param>
        void Error(string message, Exception exception, object otherProps = null);

        /// <summary>
        /// Write an ERROR message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties dictionary to be added to the log entry properties</param>
        void Error(string message, Exception exception, Dictionary<string, object> otherProps);

        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties object (Ex: new { prop1 = "test" }) </param>
        void Write(LogLevel logLevel, string message, Exception exception = null, object otherProps = null);

        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties dictionary</param>
        void Write(LogLevel logLevel, string message, Exception exception, Dictionary<string, object> otherProps);

        /// <summary>
        /// Write a log entry object directly to the instance's logging mechanism.
        /// IMPORTANT: It is HIGHLY recommended that this method is not used unless you know what you are doing.
        /// It was exposed only to allow the <see cref="AggregateLogger"/> to be built.
        /// </summary>
        /// <param name="logEntry">Log entry</param>
        void Write(LogEntry logEntry);
    }
}