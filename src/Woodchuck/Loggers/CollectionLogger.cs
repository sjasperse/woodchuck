using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Woodchuck.ContextProviders;

namespace Woodchuck.Loggers
{
    /// <summary>
    /// Logger implementation that records entries to a collection.
    /// Useful for testing.
    /// </summary>
    public class CollectionLogger : Logger
    {
        private readonly List<LogEntry> logEntries = new List<LogEntry>();

        public CollectionLogger(IContextProvider contextProvider)
            : base(contextProvider)
        {
        }

        public IEnumerable<LogEntry> LogEntries { get { return logEntries; } }

        protected override void WriteImplementation(LogEntry logEntry)
        {
            this.logEntries.Add(logEntry);
        }
    }
}