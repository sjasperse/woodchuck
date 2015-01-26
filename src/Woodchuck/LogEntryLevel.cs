using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck
{
    /// <summary>
    /// Logging Level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug log level
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Info log level
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warn log level
        /// </summary>
        Warn = 2,

        /// <summary>
        /// Error log level
        /// </summary>
        Error = 3,

        /// <summary>
        /// Fatal log level
        /// </summary>
        Fatal = 4
    }
}