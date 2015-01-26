using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Woodchuck.ContextProviders;
using Woodchuck.Loggers;

namespace Woodchuck.Tests.Loggers
{
    [TestClass]
    public class Log4NetLoggerTests
    {
        private Mock<log4net.Core.ILogger> log4netCoreLogger;
        private Log4NetLogger logger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.log4netCoreLogger = new Mock<log4net.Core.ILogger>();
            this.logger = new Log4NetLogger(new ApplicationContextProvider(), this.log4netCoreLogger.Object);
            this.logger.UseExceptionHandling = false;
        }

        [TestMethod]
        public void Log4NetLogger_Write_WritesToLog4NetCoreLogger()
        {
            this.logger.Write(LogLevel.Info, "test");

            this.log4netCoreLogger.Verify(m => m.Log(It.IsAny<log4net.Core.LoggingEvent>()));
        }

        [TestMethod]
        public void Log4NetLogger_Write_NeedMoreTests()
        {
            Assert.Inconclusive();
        }
    }
}