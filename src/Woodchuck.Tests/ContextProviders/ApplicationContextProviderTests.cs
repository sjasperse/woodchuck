using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Woodchuck.ContextProviders;

namespace Woodchuck.Tests.ContextProviders
{
    [TestClass]
    public class ApplicationContextProviderTests
    {
        [TestMethod]
        public void ApplicationContextProvider_GetContext()
        {
            var contextProvider = new ApplicationContextProvider();
            var context = contextProvider.GetCurrent();

            context.Username.Should().NotBeNullOrEmpty();
        }
    }
}