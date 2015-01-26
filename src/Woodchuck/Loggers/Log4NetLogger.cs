using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Woodchuck.ContextProviders;

namespace Woodchuck.Loggers
{
    /// <summary>
    /// Logger which writes to the log4net logging infrastructure
    /// </summary>
    public class Log4NetLogger : Logger, ILogger
    {
        #region Members

        /// <summary>
        /// log4net.Core.ILogger dependency
        /// </summary>
        private readonly log4net.Core.ILogger log4netLogger;

        #endregion Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="contextProvider"><see cref="ILoggingContextProvider"/> dependency</param>
        public Log4NetLogger(IContextProvider contextProvider)
            : base(contextProvider)
        {
            log4net.Config.XmlConfigurator.Configure();
            this.log4netLogger = log4net.Core.LoggerManager.GetLogger(this.GetType().Assembly, this.GetType().Name);
            this.LoggerName = this.GetType().Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        /// <param name="contextProvider"><see cref="ILoggingContextProvider"/> dependency</param>
        /// <param name="log4netLogger"><see cref="log4net.Core.ILogger"/> dependency</param>
        public Log4NetLogger(IContextProvider contextProvider, log4net.Core.ILogger log4netLogger)
            : base(contextProvider)
        {
            this.log4netLogger = log4netLogger;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the logger name
        /// </summary>
        public string LoggerName { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Writes the log entry to the log4net infrastructure
        /// </summary>
        /// <param name="logEntry">Log entry</param>
        protected override void WriteImplementation(LogEntry logEntry)
        {
            var log4netLevel = this.ConvertOurLogLevelToLog4NetLogLevel(logEntry.LogLevel);

            Func<string, string> getStackValue = (prop) => logEntry.EntryProperties.ContainsKey(prop) ? logEntry.EntryProperties[prop] : null;

            var logEventData = new log4net.Core.LoggingEventData()
            {
                Level = log4netLevel,
                LoggerName = this.LoggerName,
                Domain = AppDomain.CurrentDomain.FriendlyName,
                Identity = logEntry.Username,
                UserName = logEntry.Username,
                TimeStamp = DateTime.Now,
                Message = logEntry.Message,
                LocationInfo = new log4net.Core.LocationInfo(
                    getStackValue(Logger.Props.ClassingClassName),
                    getStackValue(Logger.Props.ClassingMethodName),
                    getStackValue(Logger.Props.CallingFileName),
                    getStackValue(Logger.Props.CallingLineNumber)),
                //// ExceptionString = logEntry.Exception != null ? logEntry.Exception.ToString() : null // <-- Causes additional log entries that are unwanted
            };
            logEntry.EntryProperties["LoggerName"] = this.LoggerName;

            var logEvent = new log4net.Core.LoggingEvent(logEventData);

            // include all logEntry context properties
            foreach (KeyValuePair<string, string> kvp in logEntry.ContextProperties)
            {
                logEvent.Properties[kvp.Key] = kvp.Value;

                // Do this so that any other packages using log4net will still have context properties associated with this thread.
                log4net.ThreadContext.Properties[kvp.Key] = kvp.Value;
            }

            // include all logEntry entry properties
            foreach (KeyValuePair<string, string> kvp in logEntry.EntryProperties)
            {
                logEvent.Properties[kvp.Key] = kvp.Value;
            }

            this.log4netLogger.Log(logEvent);
        }

        /// <summary>
        /// Read the method name
        /// </summary>
        /// <param name="ourLogLevel">Our <see cref="LogLevel"/> instance</param>
        /// <returns>Log4net Log Level instance</returns>
        protected virtual log4net.Core.Level ConvertOurLogLevelToLog4NetLogLevel(LogLevel ourLogLevel)
        {
            switch (ourLogLevel)
            {
                case LogLevel.Debug: return log4net.Core.Level.Debug;
                case LogLevel.Warn: return log4net.Core.Level.Warn;
                case LogLevel.Error: return log4net.Core.Level.Error;
                case LogLevel.Fatal: return log4net.Core.Level.Fatal;
                case LogLevel.Info:
                default: return log4net.Core.Level.Info;
            }
        }

        #endregion Methods
    }
}