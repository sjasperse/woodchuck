using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Woodchuck.Loggers;

namespace Woodchuck.log4netArtifacts
{
    /// <summary>
    /// Writes log4net entry as a JSON string
    /// </summary>
    public class JsonLayout : log4net.Layout.LayoutSkeleton
    {
        /// <summary>
        /// Gets the first properties that should be in the JSON string. For log readability
        /// </summary>
        public static IEnumerable<string> FirstProperties
        {
            get
            {
                return new string[]
                {
                    Logger.Props.Timestamp,
                    Logger.Props.LogLevel,
                    Logger.Props.Message
                };
            }
        }

        /// <summary>
        /// Probably to initialize the instance. Called by log4net
        /// </summary>
        public override void ActivateOptions()
        {
        }

        /// <summary>
        /// Writes to the output stream
        /// </summary>
        /// <param name="writer">Log writer</param>
        /// <param name="loggingEvent">Log event</param>
        public override void Format(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            // I don't like the performance here, but it's needed to order the first few properties, and help the json message be readable
            Dictionary<string, object> props = new Dictionary<string, object>();

            foreach (string prop in FirstProperties)
            {
                props[prop] = loggingEvent.Properties[prop];
            }

            foreach (DictionaryEntry kvp in loggingEvent.GetProperties())
            {
                string key = kvp.Key.ToString();
                if (!props.ContainsKey(key))
                {
                    props[key] = kvp.Value;
                }
            }

            string json = JsonConvert.SerializeObject(props);

            writer.WriteLine(json);
        }
    }
}