using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck
{
    public class Context
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingContext"/> class.
        /// </summary>
        public Context()
        {
            this.OtherProperties = new Dictionary<string, string>();
        }

        public string Username { get; set; }

        /// <summary>
        /// Gets dictionary of any other properties that are needed to provide context to any logging entries (host machine name, request url, etc...)
        /// </summary>
        public Dictionary<string, string> OtherProperties { get; private set; }
    }
}