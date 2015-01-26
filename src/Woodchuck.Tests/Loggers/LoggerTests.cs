using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Woodchuck.ContextProviders;
using Woodchuck.Loggers;

namespace Woodchuck.Tests.Loggers
{
    [TestClass]
    public class LoggerTests
    {
        private Mock<IContextProvider> contextProviderMock;
        private CollectionLogger logger; // using collection logger since it is a very simple, observable implementation of the underlying Logger
        private string testMessage;

        [TestInitialize]
        public void TestInitialize()
        {
            this.contextProviderMock = new Mock<IContextProvider>();
            this.contextProviderMock.Setup(m => m.GetCurrent()).Returns(new Context() { Username = System.Environment.UserName });

            this.logger = new CollectionLogger(this.contextProviderMock.Object);

            this.testMessage = "Test message " + Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void Logger_Info_ShouldWriteInfoEntry()
        {
            this.logger.Info(this.testMessage);

            this.logger.LogEntries.Should().HaveCount(1);
            this.logger.LogEntries.Single().LogLevel.Should().Be(LogLevel.Info);
            this.logger.LogEntries.Single().Message.Should().Be(this.testMessage);
        }

        [TestMethod]
        public void Logger_Debug_ShouldWriteDebugEntry()
        {
            this.logger.Debug(this.testMessage);

            this.logger.LogEntries.Should().HaveCount(1);
            this.logger.LogEntries.Single().LogLevel.Should().Be(LogLevel.Debug);
            this.logger.LogEntries.Single().Message.Should().Be(this.testMessage);
        }

        [TestMethod]
        public void Logger_Error_ShouldWriteErrorEntry()
        {
            this.logger.Error(this.testMessage, new Exception(this.testMessage));

            this.logger.LogEntries.Should().HaveCount(1);
            this.logger.LogEntries.Single().LogLevel.Should().Be(LogLevel.Error);
            this.logger.LogEntries.Single().Message.Should().Be(this.testMessage);
        }

        [TestMethod]
        public void Logger_Error_ShouldWriteLogEntryWIthException()
        {
            var ex = new Exception(this.testMessage);
            this.logger.Error(this.testMessage, ex);

            this.logger.LogEntries.Single().Exception.Should().Be(ex);
        }

        [TestMethod]
        public void Logger_Write_ShouldWriteEntryWithSaveLogLevelAndMessage()
        {
            this.logger.Write(LogLevel.Warn, this.testMessage);

            this.logger.LogEntries.Should().HaveCount(1);
            this.logger.LogEntries.Single().LogLevel.Should().Be(LogLevel.Warn);
            this.logger.LogEntries.Single().Message.Should().Be(this.testMessage);
        }

        [TestMethod]
        public void Logger_Write_WithOtherPropsObject()
        {
            this.logger.Write(LogLevel.Info, this.testMessage, otherProps: new { testMessage = this.testMessage });

            this.logger.LogEntries.Single().EntryProperties.Should().ContainKey("testMessage");
        }

        [TestMethod]
        public void Logger_Write_WithOtherPropsDictionary()
        {
            this.logger.Write(
                LogLevel.Info,
                this.testMessage,
                exception: null,
                otherPropsDict: new Dictionary<string, object>()
                {
                    { "testMessage", this.testMessage }
                }
            );

            this.logger.LogEntries.Single().EntryProperties.Should().ContainKey("testMessage");
        }

        [TestMethod]
        public void Logger_Write_ShouldLogCanDisable()
        {
            var logger = new CollectionLogger(this.contextProviderMock.Object)
                {
                    LogLevelsHandled = new[] { LogLevel.Debug }
                };

            logger.Info(this.testMessage);
            logger.Debug(this.testMessage);

            logger.LogEntries.Should().HaveCount(1);
            logger.LogEntries.Single().Message.Should().Be(this.testMessage);
            logger.LogEntries.Single().LogLevel.Should().Be(LogLevel.Debug);
        }
    }
}