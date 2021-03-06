﻿// Copyright 2015-2019 Serilog Contributors
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

// Disable ReSharper Warnings 
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.ComponentModel;
using System.Net.Http;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.ElasticEcsLogstash;
using Serilog.Sinks.ElasticEcsLogstash.BatchFormatters;
using Serilog.Sinks.ElasticEcsLogstash.Private.Network;
using Serilog.Sinks.ElasticEcsLogstash.Private.Sinks;
using Serilog.Sinks.ElasticEcsLogstash.TextFormatters;

namespace Serilog
{
    /// <summary>
    /// Class containing extension methods to <see cref="LoggerConfiguration"/>, configuring sinks
    /// sending log events over the network using HTTP.
    /// </summary>
    public static class LoggerEcsSinkConfigurationExtensions
    {
        /// <summary>
        /// Adds a non-durable sink that sends log events using HTTP POST over the network. A
        /// non-durable sink will lose data after a system or process restart.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="requestUri">The URI the request is sent to.</param>
        /// <param name="batchPostingLimit">
        /// The maximum number of events to post in a single batch. Default value is 1000.
        /// </param>
        /// <param name="queueLimit">
        /// The maximum number of events stored in the queue in memory, waiting to be posted over
        /// the network. Default value is infinitely.
        /// </param>
        /// <param name="period">
        /// The time to wait between checking for event batches. Default value is 2 seconds.
        /// </param>
        /// <param name="textFormatter">
        /// The formatter rendering individual log events into text, for example JSON. Default
        /// value is <see cref="NormalRenderedTextFormatter"/>.
        /// </param>
        /// <param name="batchFormatter">
        /// The formatter batching multiple log events into a payload that can be sent over the
        /// network. Default value is <see cref="DefaultBatchFormatter"/>.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. Default value is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <param name="httpClient">
        /// A custom <see cref="IHttpClient"/> implementation. Default value is
        /// <see cref="HttpClient"/>.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration ElasticEcsLogstash(
            this LoggerSinkConfiguration sinkConfiguration,
            string requestUri,
            int batchPostingLimit = 1000,
            int? queueLimit = null,
            TimeSpan? period = null,
            ITextFormatter textFormatter = null,
            IBatchFormatter batchFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IHttpClient httpClient = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            // Default values 
            // Emzam.Log.ElkLogProvider
            // Serilog.Sinks.ElasticEcsLogstash
            period ??= TimeSpan.FromSeconds(2);
            textFormatter ??= new NormalRenderedTextFormatter();
            batchFormatter ??= new DefaultBatchFormatter();
            httpClient ??= new DefaultHttpClient();

            var sink = queueLimit != null
                ? new ElasticEcsLogstashSink(requestUri, batchPostingLimit, queueLimit.Value, period.Value, textFormatter, batchFormatter, httpClient)
                : new ElasticEcsLogstashSink(requestUri, batchPostingLimit, period.Value, textFormatter, batchFormatter, httpClient);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        [Obsolete("Use DurableHttpUsingTimeRolledBuffers instead of this sink provider")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration DurableElasticEcsLogstash(
            this LoggerSinkConfiguration sinkConfiguration,
            string requestUri,
            string bufferPathFormat = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = null,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            ITextFormatter textFormatter = null,
            IBatchFormatter batchFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IHttpClient httpClient = null)
        {
            return DurableElasticEcsLogstashUsingTimeRolledBuffers(
                sinkConfiguration,
                requestUri,
                bufferPathFormat,
                bufferFileSizeLimitBytes,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                textFormatter,
                batchFormatter,
                restrictedToMinimumLevel,
                httpClient);
        }

        /// <summary>
        /// Adds a durable sink that sends log events using HTTP POST over the network. A durable
        /// sink will persist log events on disk in buffer files before sending them over the
        /// network, thus protecting against data loss after a system or process restart. The
        /// buffer files will use a rolling behavior defined by the time interval specified in
        /// <paramref name="bufferPathFormat"/>, i.e. a new buffer file is created every time a new
        /// interval is started. The maximum size of a file is defined by
        /// <paramref name="bufferFileSizeLimitBytes"/>, and when that limit is reached all
        /// incoming log events will be dropped until a new interval is started.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="requestUri">The URI the request is sent to.</param>
        /// <param name="bufferPathFormat">
        /// The relative or absolute path format for a set of files that will be used to buffer
        /// events until they can be successfully sent over the network. Default value is
        /// "Buffer-{Date}.json". To use file rotation that is on an 30 or 60 minute interval pass
        /// "Buffer-{HalfHour}.json" or "Buffer-{Hour}.json".
        /// </param>
        /// <param name="bufferFileSizeLimitBytes">
        /// The approximate maximum size, in bytes, to which a buffer file for a specific time interval will be
        /// allowed to grow. By default no limit will be applied.
        /// </param>
        /// <param name="retainedBufferFileCountLimit">
        /// The maximum number of buffer files that will be retained, including the current buffer
        /// file. Under normal operation only 2 files will be kept, however if the log server is
        /// unreachable, the number of files specified by <paramref name="retainedBufferFileCountLimit"/>
        /// will be kept on the file system. For unlimited retention, pass null. Default value is 31.
        /// </param>
        /// <param name="batchPostingLimit">
        /// The maximum number of events to post in a single batch. Default value is 1000.
        /// </param>
        /// <param name="period">
        /// The time to wait between checking for event batches. Default value is 2 seconds.
        /// </param>
        /// <param name="textFormatter">
        /// The formatter rendering individual log events into text, for example JSON. Default
        /// value is <see cref="NormalRenderedTextFormatter"/>.
        /// </param>
        /// <param name="batchFormatter">
        /// The formatter batching multiple log events into a payload that can be sent over the
        /// network. Default value is <see cref="DefaultBatchFormatter"/>.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. Default value is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <param name="httpClient">
        /// A custom <see cref="IHttpClient"/> implementation. Default value is
        /// <see cref="HttpClient"/>.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration DurableElasticEcsLogstashUsingTimeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            string requestUri,
            string bufferPathFormat = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = null,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            ITextFormatter textFormatter = null,
            IBatchFormatter batchFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IHttpClient httpClient = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            // Default values
            period ??= TimeSpan.FromSeconds(2);
            textFormatter ??= new NormalRenderedTextFormatter();
            batchFormatter ??= new DefaultBatchFormatter();
            httpClient ??= new DefaultHttpClient();

            var sink = new TimeRolledDurableElasticEcsLogstashSink(
                requestUri,
                bufferPathFormat,
                bufferFileSizeLimitBytes,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period.Value,
                textFormatter,
                batchFormatter,
                httpClient);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a durable sink that sends log events using HTTP POST over the network. A durable
        /// sink will persist log events on disk in buffer files before sending them over the
        /// network, thus protecting against data loss after a system or process restart. The
        /// buffer files will use a rolling behavior defined by the file size specified in
        /// <paramref name="bufferFileSizeLimitBytes"/>, i.e. a new buffer file is created when
        /// current has passed its limit. The maximum number of retained files is defined by
        /// <paramref name="retainedBufferFileCountLimit"/>, and when that limit is reached the
        /// oldest file is dropped to make room for a new.
        /// </summary>
        /// <param name="sinkConfiguration">The logger configuration.</param>
        /// <param name="requestUri">The URI the request is sent to.</param>
        /// <param name="bufferBaseFileName">
        /// The relative or absolute path for a set of files that will be used to buffer events
        /// until they can be successfully transmitted across the network. Individual files will be
        /// created using the pattern "<paramref name="bufferBaseFileName"/>*.json", which should
        /// not clash with any other file names in the same directory. Default value is "Buffer".
        /// </param>
        /// <param name="bufferFileSizeLimitBytes">
        /// The approximate maximum size, in bytes, to which a buffer file will be allowed to grow.
        /// For unrestricted growth, pass null. The default is 1 GB. To avoid writing partial
        /// events, the last event within the limit will be written in full even if it exceeds the
        /// limit.
        /// </param>
        /// <param name="retainedBufferFileCountLimit">
        /// The maximum number of buffer files that will be retained, including the current buffer
        /// file. Under normal operation only 2 files will be kept, however if the log server is
        /// unreachable, the number of files specified by <paramref name="retainedBufferFileCountLimit"/>
        /// will be kept on the file system. For unlimited retention, pass null. Default value is 31.
        /// </param>
        /// <param name="batchPostingLimit">
        /// The maximum number of events to post in a single batch. Default value is 1000.
        /// </param>
        /// <param name="period">
        /// The time to wait between checking for event batches. Default value is 2 seconds.
        /// </param>
        /// <param name="textFormatter">
        /// The formatter rendering individual log events into text, for example JSON. Default
        /// value is <see cref="NormalRenderedTextFormatter"/>.
        /// </param>
        /// <param name="batchFormatter">
        /// The formatter batching multiple log events into a payload that can be sent over the
        /// network. Default value is <see cref="DefaultBatchFormatter"/>.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        /// The minimum level for events passed through the sink. Default value is
        /// <see cref="LevelAlias.Minimum"/>.
        /// </param>
        /// <param name="httpClient">
        /// A custom <see cref="IHttpClient"/> implementation. Default value is
        /// <see cref="HttpClient"/>.
        /// </param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        public static LoggerConfiguration DurableElasticEcsLogstashUsingFileSizeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            string requestUri,
            string bufferBaseFileName = "Buffer",
            long? bufferFileSizeLimitBytes = 1024 * 1024 * 1024,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            ITextFormatter textFormatter = null,
            IBatchFormatter batchFormatter = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IHttpClient httpClient = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            // Default values
            period ??= TimeSpan.FromSeconds(2);
            textFormatter ??= new NormalRenderedTextFormatter();
            batchFormatter ??= new DefaultBatchFormatter();
            httpClient ??= new DefaultHttpClient();

            var sink = new FileSizeRolledDurableElasticEcsLogstashSink(
                requestUri,
                bufferBaseFileName,
                bufferFileSizeLimitBytes,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period.Value,
                textFormatter,
                batchFormatter,
                httpClient);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
