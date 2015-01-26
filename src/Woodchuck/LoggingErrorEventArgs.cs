using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck
{
    public class LoggingErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
    }
}