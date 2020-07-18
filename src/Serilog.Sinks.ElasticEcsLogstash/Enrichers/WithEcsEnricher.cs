using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Enrichers.Private.Ecs;
using Serilog.Events;

namespace Serilog.Enrichers
{
    public class WithEcsEnricher : ILogEventEnricher
    {
        private readonly HttpContext _context;

        public WithEcsEnricher()
            : this(null)
        {
        }

        public WithEcsEnricher(HttpContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            var ecsProperties = ConvertToEcs(_context, logEvent, propertyFactory);

            var properties = logEvent.Properties.Select(x => x.Key).ToList();
            foreach (var property in properties)
                logEvent.RemovePropertyIfPresent(property);

            foreach (var property in ecsProperties.OrderBy(x => x.Key))
                logEvent.AddPropertyIfAbsent(property.Value);
        }

        private static Dictionary<string, LogEventProperty> ConvertToEcs(HttpContext context, LogEvent e, ILogEventPropertyFactory propertyFactory)
        {
            try
            {
                var ecsModel = LogEventToEcsConverter.ConvertToEcs(context, e);
                var properties = MapToDictionary(ecsModel, null, propertyFactory);
                return properties;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static Dictionary<string, LogEventProperty> MapToDictionary(object source, string name, ILogEventPropertyFactory propertyFactory)
        {
            var dictionary = new Dictionary<string, LogEventProperty>();
            MapToDictionaryInternal(dictionary, source, name, propertyFactory);
            return dictionary;
        }

        private static void MapToDictionaryInternal(
            IDictionary<string, LogEventProperty> dictionary, object source, string name, ILogEventPropertyFactory propertyFactory)
        {
            var properties = source.GetType().GetProperties();
            foreach (var p in properties)
            {
                var value = p.GetValue(source, null);
                if (value == null) continue;

                string key;
                if (string.IsNullOrEmpty(name))
                    key = p.Name;
                else
                    key = name + "." + p.Name;

                var valueType = value.GetType();

                if (valueType.IsPrimitive || valueType == typeof(string) ||
                    valueType == typeof(DateTime) || valueType == typeof(DateTimeOffset))
                {
                    dictionary[key] = propertyFactory.CreateProperty(key, value);
                }
                else if (value is IEnumerable)
                {
                    var i = 0;
                    foreach (var o in (IEnumerable) value)
                    {
                        if (o is string || o is DateTime || o is DateTimeOffset)
                        {
                            dictionary[key + "[" + i + "]"] = propertyFactory.CreateProperty(key + "[" + i + "]", o);
                            i++;
                            continue;
                        }

                        MapToDictionaryInternal(dictionary, o, key + "[" + i + "]", propertyFactory);
                        i++;
                    }
                }
                else
                {
                    MapToDictionaryInternal(dictionary, value, key, propertyFactory);
                }
            }
        }
    }
}