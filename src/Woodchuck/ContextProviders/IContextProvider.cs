using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck.ContextProviders
{
    /// <summary>
    /// Provider which allows access to the current context
    /// </summary>
    public interface IContextProvider
    {
        /// <summary>
        /// Gets the current context
        /// </summary>
        /// <returns>Current context</returns>
        Context GetCurrent();
    }
}