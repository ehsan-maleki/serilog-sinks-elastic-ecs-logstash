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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.ElasticEcsLogstash.TextFormatters
{
    /// <summary>
    /// JSON formatter serializing log events into a normal format with its data normalized. The
    /// lack of a rendered message means improved network load compared to
    /// <see cref="NormalRenderedTextFormatter"/>. Often this formatter is complemented with a log
    /// server that is capable of rendering the messages of the incoming log events.
    /// </summary>
    /// <seealso cref="NormalRenderedTextFormatter" />
    /// <seealso cref="CompactTextFormatter" />
    /// <seealso cref="CompactRenderedTextFormatter" />
    /// <seealso cref="NamespacedTextFormatter" />
    /// <seealso cref="ITextFormatter" />
    public class NormalTextFormatter : ITextFormatter
    {
        /// <summary>
        /// Gets or sets a value indicating whether the message is rendered into JSON.
        /// </summary>
        protected bool IsRenderingMessage { get; set; }

        /// <inheritdoc />
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

            if (logEvent.Properties.Count <= 0)
                return;

            WriteProperties(logEvent.Properties, output);
            //WriteChildren("", "", logEvent.Properties.ToList(), output);
        }

        #region Old Formatter

        private static void WriteProperties(
            IReadOnlyDictionary<string, LogEventPropertyValue> properties,
            TextWriter output)
        {
            output.Write('{');

            var precedingDelimiter = "";

            foreach (var property in properties.Where(x => x.Key.IndexOf('[') == -1))
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                var key = property.Key.ToUnderscoreCase();
                if (property.Key.ToLower() == "timestamp")
                    key = "@timestamp";

                JsonValueFormatter.WriteQuotedJsonString(key, output);
                output.Write(':');
                ValueFormatter.Instance.Format(property.Value, output);
            }

            foreach (var arraySet in properties.Where(x => x.Key.IndexOf('[') >= 0).GroupBy(g => g.Key.Substring(0, g.Key.IndexOf('['))))
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(arraySet.Key, output);
                output.Write(':');
                output.Write('[');
                WriteArray(arraySet.AsEnumerable(), output);
                output.Write(']');
            }

            output.Write('}');
        }

        private static void WriteArray(IEnumerable<KeyValuePair<string, LogEventPropertyValue>> properties, TextWriter output)
        {
            var precedingDelimiter = "";

            foreach (var property in properties.ToList())
            {
                output.Write(precedingDelimiter);
                precedingDelimiter = ",";
                ValueFormatter.Instance.Format(property.Value, output);
            }
        }

        #endregion

        private static void WriteChildren(string delimiter, string parent, List<KeyValuePair<string, LogEventPropertyValue>> properties,
            TextWriter output)
        {
            output.Write('{');
            var fullParent = string.IsNullOrEmpty(parent) ? "xXx" : parent + ".";

            // ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
            // Return if empty
            //
            if (!properties.Any())
            {
                output.Write('}');
                return;
            }

            // ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
            // Rendering leaves
            //
            var leaves = properties.Where(x =>
                x.Key.Replace(fullParent, "").IndexOf('.') == -1 &&
                x.Key.Replace(fullParent, "").IndexOf('[') == -1).ToList();

            if (leaves.Any() && WriteLeaves(delimiter, parent, leaves, output))
                delimiter = ",";

            // ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
            // Rendering arrays 
            //
            var arrays = properties.Where(x =>
                x.Key.Replace(fullParent, "").IndexOf('.') == -1 &&
                x.Key.Replace(fullParent, "").IndexOf('[') >= 0).ToList();
            var arrayAdded = arrays.Any() && WriteArrays(delimiter, parent, arrays, output);
            if (delimiter != "," && arrayAdded)
                delimiter = ",";

            // ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~
            // Rendering objects 
            //
            var objects = properties.Where(x => x.Key.Replace(fullParent, "").IndexOf('.') >= 0)
                .GroupBy(g => g.Key.Replace(fullParent, "")
                    .Substring(0, g.Key.Replace(fullParent, "")
                        .IndexOf('.')))
                .ToList();
            if (objects.Any())
                foreach (var objectSet in objects)
                {
                    output.Write(delimiter);
                    delimiter = ",";

                    JsonValueFormatter.WriteQuotedJsonString(objectSet.Key.ToUnderscoreCase(), output);
                    output.Write(':');

                    var currentParent = fullParent == "xXx" ? objectSet.Key : fullParent + objectSet.Key;
                    WriteChildren("", currentParent, objectSet.AsEnumerable().ToList(), output);
                }

            output.Write('}');
        }

        /// <summary>
        ///  Rendering x=y properties
        /// These properties have no any children
        /// These properties are not array
        /// </summary>
        /// <param name="delimiter">Json values separator</param>
        /// <param name="parent">Full parents key and dotted format, Such as : Emzam.NewUsers.Registered</param>
        /// <param name="properties">List of leaf children</param>
        /// <param name="output">TextWriter to write json string</param>
        private static bool WriteLeaves(string delimiter, string parent, List<KeyValuePair<string, LogEventPropertyValue>> properties,
            TextWriter output)
        {
            var fullParent = string.IsNullOrEmpty(parent) ? "xXx" : parent + ".";

            foreach (var property in properties)
            {
                var key = property.Key.Replace(fullParent, "").ToUnderscoreCase();
                if (key.ToLower() == "timestamp")
                    key = "@timestamp";

                output.Write(delimiter);
                delimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(key, output);
                output.Write(':');
                ValueFormatter.Instance.Format(property.Value, output);
            }

            return delimiter != "";
        }

        private static bool WriteArrays(string delimiter, string parent, List<KeyValuePair<string, LogEventPropertyValue>> properties,
            TextWriter output)
        {
            var fullParent = string.IsNullOrEmpty(parent) ? "xXx" : parent + ".";

            foreach (var arraySet in properties.GroupBy(g => g.Key.Replace(fullParent, "")
                .Substring(0, g.Key.Replace(fullParent, "").IndexOf('['))))
            {
                var property = arraySet.First();

                output.Write(delimiter);
                delimiter = ",";

                JsonValueFormatter.WriteQuotedJsonString(arraySet.Key.ToUnderscoreCase(), output);
                output.Write(':');
                output.Write('[');
                var itemsDelimiter = "";

                var objectKey = fullParent +
                                property.Key.Replace(fullParent, "")
                                    .Substring(0, property.Key
                                        .Replace(fullParent, "")
                                        .IndexOf(']'));
                var objectArray = arraySet.Where(x => x.Key.StartsWith(objectKey)).ToList();
                if (objectArray.Count > 1)
                {
                    WriteChildren(delimiter, objectKey + "]", objectArray, output);
                    output.Write(']');
                    continue;
                }

                foreach (var item in arraySet.AsEnumerable())
                {
                    output.Write(itemsDelimiter);
                    itemsDelimiter = ",";
                    ValueFormatter.Instance.Format(item.Value, output);
                }
                
                output.Write(']');
            }

            return delimiter != "";
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