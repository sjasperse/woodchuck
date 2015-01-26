using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Woodchuck.ContextProviders;

namespace Woodchuck.Loggers
{
    public abstract class Logger : ILogger
    {
        #region Members

        /// <summary>
        /// <see cref="ILoggingContextProvider"/> dependency
        /// </summary>
        private readonly IContextProvider contextProvider;

        /// <summary>
        /// Indicates if exceptions should be silently handled
        /// </summary>
        private bool handleExceptions = true;

        #endregion Members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="contextProvider"><see cref="IContextProvider"/> dependency</param>
        public Logger(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider;
            this.LogLevelsHandled = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>();
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Event raise upon logging error
        /// </summary>
        public static event EventHandler<LoggingErrorEventArgs> OnError;

        /// <summary>
        /// Event called after an entry was successfully written.
        /// </summary>
        public event EventHandler<LogEntryEventArgs> OnEntryWritten;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether exception handling is used
        /// </summary>
        public bool UseExceptionHandling
        {
            get
            {
                return this.handleExceptions;
            }

            set
            {
                this.handleExceptions = value;
            }
        }

        /// <summary>
        /// Gets or sets a list of log levels to be handled by this instance.
        /// For performance reason, ShouldLog takes this into account to filter what gets sent down.
        /// This same thing can be done in log4net - but this shortcuts all the context building
        /// </summary>
        public IEnumerable<LogLevel> LogLevelsHandled { get; set; }

        #endregion Properties

        #region Static Members

        /// <summary>
        /// Allows an exception to be raised
        /// </summary>
        /// <param name="source">Source instance</param>
        /// <param name="ex">Exception instance</param>
        public static void RaiseError(object source, Exception ex)
        {
            if (Logger.OnError != null)
            {
                Logger.OnError(source, new LoggingErrorEventArgs() { Exception = ex });
            }
        }

        #endregion Static Members

        #region Methods

        /// <summary>
        /// Write an INFO message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties object to be added to the log entry properties (Ex: new { prop1 = "test" }) </param>
        public void Info(string message, object otherProps = null)
        {
            this.Write(LogLevel.Info, message, null, otherProps);
        }

        /// <summary>
        /// Write an INFO message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties dictionary to be added to the log entry properties</param>
        public void Info(string message, Dictionary<string, object> otherProps)
        {
            this.Write(LogLevel.Info, message, null, otherProps);
        }

        /// <summary>
        /// Write an DEBUG message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties object to be added to the log entry properties (Ex: new { prop1 = "test" }) </param>
        public void Debug(string message, object otherProps = null)
        {
            this.Write(LogLevel.Debug, message, null, otherProps);
        }

        /// <summary>
        /// Write an DEBUG message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="otherProps">Other properties dictionary to be added to the log entry properties</param>
        public void Debug(string message, Dictionary<string, object> otherProps)
        {
            this.Write(LogLevel.Debug, message, null, otherProps);
        }

        /// <summary>
        /// Write an ERROR message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties object to be added to the log entry properties (Ex: new { prop1 = "test" }) </param>
        public void Error(string message, Exception exception, object otherProps = null)
        {
            this.Write(LogLevel.Error, message, exception, otherProps);
        }

        /// <summary>
        /// Write an ERROR message to the log
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties dictionary to be added to the log entry properties</param>
        public void Error(string message, Exception exception, Dictionary<string, object> otherProps)
        {
            this.Write(LogLevel.Error, message, exception, otherProps);
        }

        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties object (Ex: new { prop1 = "test" }) </param>
        public void Write(LogLevel logLevel, string message, Exception exception = null, object otherProps = null)
        {
            this.Try(() =>
                {
                    if (this.ShouldLog(logLevel))
                    {
                        Dictionary<string, object> props = new Dictionary<string, object>();

                        if (otherProps != null)
                        {
                            foreach (var prop in otherProps.GetType().GetProperties())
                            {
                                var value = prop.GetValue(otherProps, null);
                                string valueStr = null;
                                if (value != null)
                                {
                                    valueStr = value.ToString();
                                }

                                props[prop.Name] = valueStr;
                            }
                        }

                        this.Write(logLevel, message, exception, props);
                    }
                });
        }

        /// <summary>
        /// Write log entry
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherPropsDict">Other properties dictionary</param>
        public void Write(LogLevel logLevel, string message, Exception exception, Dictionary<string, object> otherPropsDict)
        {
            this.Try(() =>
            {
                if (this.ShouldLog(logLevel))
                {
                    this.Write(this.CreateLogEntry(logLevel, message, exception, otherPropsDict));
                }
            });
        }

        /// <summary>
        /// Write a log entry object directly to the instance's logging mechanism.
        /// IMPORTANT: It is HIGHLY recommended that this method is not used unless you know what you are doing.
        /// It was exposed only to allow the <see cref="AggregateLogger"/> to be built.
        /// </summary>
        /// <param name="logEntry">Log entry</param>
        public void Write(LogEntry logEntry)
        {
            this.Try(() =>
                {
                    this.WriteImplementation(logEntry);

                    if (this.OnEntryWritten != null)
                    {
                        this.OnEntryWritten(this, new LogEntryEventArgs() { LogEntry = logEntry });
                    }
                });
        }

        /// <summary>
        /// Check if this instance wants to log this log level
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <returns>True if should be logged.</returns>
        public virtual bool ShouldLog(LogLevel logLevel)
        {
            return this.LogLevelsHandled != null
                && this.LogLevelsHandled.Contains(logLevel);
        }

        /// <summary>
        /// Write a log entry to the inheriting class's implementation
        /// </summary>
        /// <param name="logEntry">Log entry</param>
        protected abstract void WriteImplementation(LogEntry logEntry);

        /// <summary>
        /// Builds a new <see cref="LogEntry"/> object based on the provided arguments
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Log message</param>
        /// <param name="exception">Exception instance</param>
        /// <param name="otherProps">Other properties dictionary</param>
        /// <returns>new <see cref="LogEntry"/> object</returns>
        protected virtual LogEntry CreateLogEntry(LogLevel logLevel, string message, Exception exception = null, Dictionary<string, object> otherProps = null)
        {
            LogEntry logEntry = new LogEntry()
            {
                Timestamp = DateTime.Now,
                LogLevel = logLevel,
                Message = message,
                Exception = exception
            };

            // entry-level properties
            logEntry.EntryProperties[Props.Timestamp] = logEntry.Timestamp.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            logEntry.EntryProperties[Props.LogLevel] = logEntry.LogLevel.ToString();
            logEntry.EntryProperties[Props.Message] = logEntry.Message;
            if (logEntry.Exception != null)
            {
                logEntry.EntryProperties[Props.Exception] = logEntry.Exception.ToString();

                foreach (DictionaryEntry kvp in logEntry.Exception.Data)
                {
                    if (kvp.Key != null && kvp.Value != null)
                    {
                        logEntry.EntryProperties[string.Concat(Props.ExceptionDataPrefix, kvp.Key.ToString())] = kvp.Value.ToString();
                    }
                }
            }

            if (otherProps != null)
            {
                foreach (KeyValuePair<string, object> kvp in otherProps)
                {
                    logEntry.EntryProperties[kvp.Key] = kvp.Value != null ? kvp.Value.ToString() : null;
                }
            }

            // system-level properties
            logEntry.ContextProperties[Props.MachineName] = System.Environment.MachineName;
            logEntry.ContextProperties[Props.ProcessId] = Process.GetCurrentProcess().Id.ToString();
            this.AddStackInformationProperties(logEntry);

            // context-level properties
            Context context = this.contextProvider.GetCurrent();
            if (context != null)
            {
                logEntry.Username = context.Username;
                logEntry.ContextProperties[Props.Username] = context.Username;

                foreach (KeyValuePair<string, string> kvp in context.OtherProperties)
                {
                    logEntry.ContextProperties[kvp.Key] = kvp.Value;
                }
            }

            return logEntry;
        }

        /// <summary>
        /// Gives back the first stack frame outside of the logging framework.
        /// Exposed and overridable in case other custom loggers are implemented and need to change how this is acquired.
        /// </summary>
        /// <returns><see cref="StackFrame"/></returns>
        protected virtual StackFrame GetFirstReleventStackFrame()
        {
            string thisNamespace = this.GetType().Namespace;
            StackTrace stack = new StackTrace();
            return stack.GetFrames().First(m => !m.GetMethod().DeclaringType.FullName.StartsWith(thisNamespace));
        }

        private void AddStackInformationProperties(LogEntry logEntry)
        {
            StackFrame frame = this.GetFirstReleventStackFrame();
            MethodBase method = frame.GetMethod();

            // Get the Assembly type to access its metadata.
            Type reflectedType = method.ReflectedType;
            Assembly assembly = reflectedType.Assembly;
            int callingLineNumber = 0;
            string program = "unknown";
            string className = "unknown";
            string callingFileName = "unknown";
            string methodName = "unknown";
            string appDomain = "unknown";

            if (frame is StackFrame && frame != null && !object.Equals(frame, default(StackFrame)))
            {
                method = frame.GetMethod();
                if (method is MethodBase && method != null && !object.Equals(method, default(MethodBase)))
                {
                    reflectedType = method.ReflectedType;
                    if (reflectedType != null)
                    {
                        assembly = reflectedType.Assembly;
                        appDomain = reflectedType.AssemblyQualifiedName;
                        program = reflectedType.Name;
                        className = reflectedType.FullName;
                    }

                    methodName = method.Name;
                    callingLineNumber = frame.GetFileLineNumber();
                    callingFileName = method.Module.Name;
                }
            }

            // can't get the assembly version through the custom attributes, so do it here.
            string appAssemblyVersion = assembly.GetName().Version.ToString();
            string appTitle = null;
            string appDescription = null;
            string appCompany = null;
            string appAssemblyFileVersion = null;

            // Iterate through the attributes for the assembly.
            foreach (Attribute attr in Attribute.GetCustomAttributes(assembly))
            {
                if (attr.GetType() == typeof(AssemblyTitleAttribute))
                {
                    appTitle = ((AssemblyTitleAttribute)attr).Title;
                }
                else if (attr.GetType() == typeof(AssemblyDescriptionAttribute))
                {
                    appDescription = ((AssemblyDescriptionAttribute)attr).Description;
                }
                else if (attr.GetType() == typeof(AssemblyCompanyAttribute))
                {
                    appCompany = ((AssemblyCompanyAttribute)attr).Company;
                }
                else if (attr.GetType() == typeof(AssemblyFileVersionAttribute))
                {
                    appAssemblyFileVersion = ((AssemblyFileVersionAttribute)attr).Version;
                }
            }

            logEntry.EntryProperties[Props.ClassingClassName] = className;
            logEntry.EntryProperties[Props.ClassingMethodName] = methodName;
            logEntry.EntryProperties[Props.CallingFileName] = callingFileName;
            logEntry.EntryProperties[Props.CallingLineNumber] = callingLineNumber.ToString(CultureInfo.InvariantCulture);

            logEntry.ContextProperties[Props.AssemblyCompany] = appCompany;
            logEntry.ContextProperties[Props.AssemblyVersion] = appAssemblyVersion;
            logEntry.ContextProperties[Props.AssemblyFileVersion] = appAssemblyFileVersion;
            logEntry.ContextProperties[Props.AssemblyTitle] = appTitle;
            logEntry.ContextProperties[Props.AssemblyDescription] = appDescription;
        }

        /// <summary>
        /// Common exception handling for logging exceptions
        /// </summary>
        /// <param name="action">Method to be performed inside try/catch protection</param>
        private void Try(Action action)
        {
            if (this.UseExceptionHandling)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    RaiseError(this, ex);
                }
            }
            else
            {
                action();
            }
        }

        #endregion Methods

        #region Sub-classes

        /// <summary>
        /// Contains constant property names
        /// </summary>
        public static class Props
        {
            /// <summary>
            /// Timestamp property name
            /// </summary>
            public const string Timestamp = "timestamp";

            /// <summary>
            /// LogLevel property name
            /// </summary>
            public const string LogLevel = "loglevel";

            /// <summary>
            /// Message property name
            /// </summary>
            public const string Message = "message";

            /// <summary>
            /// Exception property name
            /// </summary>
            public const string Exception = "exception";

            /// <summary>
            /// Field to prefix all exception data values with
            /// </summary>
            public const string ExceptionDataPrefix = "exceptionData_";

            /// <summary>
            /// MachineName property name
            /// </summary>
            public const string MachineName = "host";

            /// <summary>
            /// ProcessId property name
            /// </summary>
            public const string ProcessId = "processId";

            /// <summary>
            /// Username property name
            /// </summary>
            public const string Username = "username";

            /// <summary>
            /// Assembly company
            /// </summary>
            public const string AssemblyCompany = "assemblyCompany";

            /// <summary>
            /// Assembly version
            /// </summary>
            public const string AssemblyVersion = "assemblyVersion";

            /// <summary>
            /// Assembly file version
            /// </summary>
            public const string AssemblyFileVersion = "assemblyFileVersion";

            /// <summary>
            /// Assembly title
            /// </summary>
            public const string AssemblyTitle = "assemblyTitle";

            /// <summary>
            /// Assembly description
            /// </summary>
            public const string AssemblyDescription = "assemblyDescription";

            /// <summary>
            /// Calling class name
            /// </summary>
            public const string ClassingClassName = "callingClassName";

            /// <summary>
            /// Calling method name
            /// </summary>
            public const string ClassingMethodName = "callingMethodName";

            /// <summary>
            /// Calling file name
            /// </summary>
            public const string CallingFileName = "callingFileName";

            /// <summary>
            /// Calling line number
            /// </summary>
            public const string CallingLineNumber = "callingLineNumber";
        }

        #endregion Sub-classes
    }
}