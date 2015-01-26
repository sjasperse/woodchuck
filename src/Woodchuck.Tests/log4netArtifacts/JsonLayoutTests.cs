using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Woodchuck.log4netArtifacts;

namespace Woodchuck.Tests.log4netArtifacts
{
    [TestClass]
    public class JsonLayoutTests
    {
        private log4net.Core.LoggingEvent loggingEvent;

        [TestInitialize]
        public void TestInitialize()
        {
            log4net.Core.LoggingEventData data = new log4net.Core.LoggingEventData()
            {
                Message = "test message",
                Level = log4net.Core.Level.Debug,
                LocationInfo = new log4net.Core.LocationInfo("class", "method", "file", "lineNum")
            };
            this.loggingEvent = new log4net.Core.LoggingEvent(data);
        }

        [TestMethod]
        public void JsonLayout_OutputsValidJson()
        {
            this.loggingEvent.Properties.Clear();
            this.loggingEvent.Properties["testKey"] = "testValue";

            string jsonStr = this.LogEventAndReturnString();
            JObject json = JObject.Parse(jsonStr);

            json.Value<string>("testKey").Should().Be("testValue");
        }

        [TestMethod]
        public void JsonLayout_JsonFieldsAreSorted()
        {
            string jsonStr = this.LogEventAndReturnString();
            JObject json = JObject.Parse(jsonStr);

            var child = json.First;
            foreach (var orderedProp in JsonLayout.FirstProperties)
            {
                child.Path.Should().Be(orderedProp);

                child = child.Next;
            }
        }

        private string LogEventAndReturnString()
        {
            JsonLayout arcsightLayout = new JsonLayout();

            StringBuilder logLine = new StringBuilder();

            using (var sw = new StringWriter(logLine))
            {
                arcsightLayout.Format(sw, this.loggingEvent);
            }

            return logLine.ToString();
        }
    }
}