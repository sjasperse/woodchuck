using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck
{
    public class LogEntryEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the <see cref="LogEntry"/> this event relates to
        /// </summary>
        public LogEntry LogEntry { get; set; }
    }
}