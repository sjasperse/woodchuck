using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck
{
    public class LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        public LogEntry()
        {
            this.EntryProperties = new Dictionary<string, string>();
            this.ContextProperties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets Time stamps
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets Log level
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets Log message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets Log exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets Current username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets Additional log properties for this entry.
        /// These will override Context properties.
        /// Examples: username (if different from context), relevant RSL POST arguments, etc...
        /// </summary>
        public Dictionary<string, string> EntryProperties { get; private set; }

        /// <summary>
        /// Gets Additional log properties that ware context scope (not entry-specific, but properties relating to machine name, process, web request, etc..)
        /// </summary>
        public Dictionary<string, string> ContextProperties { get; private set; }
    }
}