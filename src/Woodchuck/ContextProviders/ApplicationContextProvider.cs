using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Woodchuck.ContextProviders
{
    public class ApplicationContextProvider : IContextProvider
    {
        private Context currentContext;

        public Context GetCurrent()
        {
            if (this.currentContext == null)
            {
                this.currentContext = new Context()
                {
                    Username = string.Concat(System.Environment.UserDomainName, @"\", System.Environment.UserName)
                };
            }

            return this.currentContext;
        }
    }
}