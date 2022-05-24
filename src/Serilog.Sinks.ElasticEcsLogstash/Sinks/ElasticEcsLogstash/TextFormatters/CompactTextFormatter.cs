// Copyright 2015-2019 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.ElasticEcsLogstash.TextFormatters
{
    /// <summary>
    /// JSON formatter serializing log events with minimizing size as a priority and normalizing
    /// its data. The lack of a rendered message means even smaller network load compared to
    /// <see cref="CompactRenderedTextFormatter"/> and should be used in situations where bandwidth
    /// is of importance. Often this formatter is complemented with a log server that is capable of
    /// rendering the messages of the incoming log events.
    /// </summary>
    /// <seealso cref="NormalTextFormatter" />
    /// <seealso cref="NormalRenderedTextFormatter" />
    /// <seealso cref="CompactRenderedTextFormatter" />
    /// <seealso cref="NamespacedTextFormatter" />
    /// <seealso cref="ITextFormatter" />
    public class CompactTextFormatter : ITextFormatter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the message is rendered into JSON.
        /// </summary>
        protected bool IsRenderingMessage { get; set; }

        /// <summary>
        /// Format the log event into the output.
        /// </summary>
        /// <param name="logEvent">The event to format.</param>
        /// <param name="output">The output.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            try
            {
                var buffer = new StringWriter();
                FormatContent(logEvent, buffer);

                // If formatting was successful, write to output
                output.WriteLine(buffer.ToString());
            }
            catch (Exception e)
            {
                LogNonFormattableEvent(logEvent, e);
            }
        }

        private void FormatContent(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            output.Write('{');
            
            foreach (var property in logEvent.Properties)
            {
                var name = property.Key;
                if (name.Length > 0 && name[0] == '@')
                {
                    // Escape first '@' by doubling
                    name = '@' + name;
                }

                output.Write(',');
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(':');
                ValueFormatter.Instance.Format(property.Value, output);
            }
            
            output.Write('}');
        }

        private static void LogNonFormattableEvent(LogEvent logEvent, Exception e)
        {
            SelfLog.WriteLine(
                "Event at {0} with message template {1} could not be formatted into JSON and will be dropped: {2}",
                logEvent.Timestamp.ToString("o"),
                logEvent.MessageTemplate.Text,
                e);
        }
    }
}
