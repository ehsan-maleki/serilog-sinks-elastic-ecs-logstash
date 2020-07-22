using System;
using System.Collections.Generic;
using System.Configuration;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using Serilog.Core;

namespace Emzam.Log.ElkLogProvider
{
    public class ElkLogProvider : ILogProvider
    {
        public LogApplicationModel _application { get; private set; }

        public Logger _logger { get; }

        public ElkLogProvider(IHttpContextAccessor context, string logstashUrl = null, 
            LogApplicationModel application = null)
        {
            _application = application ?? new LogApplicationModel();
            
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithEcs(context)
                .WriteTo.ElasticEcsLogstash(logstashUrl ?? "http://localhost:8080")
                .CreateLogger();
        }

        public void SetApplication(LogApplicationModel application)
            => _application = new LogApplicationModel(application);
                
        public void LogInformation(string category, string name, Dictionary<string, string> payload)
        {
            Log(LogCategories.Information, category, name, payload: payload, severity: Severities.Low);
        }

        public void LogDebug(string name, Dictionary<string, string> payload, string category = "Default Logs")
        {
            Log(LogCategories.Debug, category, name, payload: payload);
        }

        public void LogWarning(string name, Dictionary<string, string> payload, string category = "Default Logs")
        {
            Log(LogCategories.Warning, category, name, payload: payload, severity: Severities.High);
        }

        public void LogAudit(string name, Dictionary<string, string> payload, string category = "Default Logs")
        {
            Log(LogCategories.Audit, category, name, payload: payload);
        }

        public void LogCritical(string name, Dictionary<string, string> payload, string category = "Default Logs")
        {
            Log(LogCategories.Notice, category, name, payload: payload, severity: Severities.Critical);
        }

        public void LogFetal(string name, Dictionary<string, string> payload, string category = "Default Logs")
        {
            Log(LogCategories.Fetal, category, name, payload: payload, severity: Severities.Fetal);
        }

        public void LogError(string name, Exception exception, Dictionary<string, string> payload, string category = "Default Logs",
            Severities severity = Severities.High)
        {
            Log(LogCategories.Error, category, name, exception, payload, severity);
        }
         
        private void Log(LogCategories category, string kind, string name, Exception exception = null, 
            Dictionary<string, string> payload = null, Severities severity = Severities.Normal)
        {
            if (string.IsNullOrEmpty(kind)) 
                throw new ArgumentNullException(nameof(kind));
            
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException(nameof(name));
            
            try
            {
                using (LogContext.PushProperty("ApplicationId", _application.Id))
                using (LogContext.PushProperty("ApplicationName", _application.Name))
                using (LogContext.PushProperty("ApplicationType", _application.Type))
                using (LogContext.PushProperty("ApplicationVersion", _application.Version))
                using (LogContext.PushProperty("ServerName", _application.Server))
                using (LogContext.PushProperty("ActionId", _application.Id))
                using (LogContext.PushProperty("ActionCategory", category))
                using (LogContext.PushProperty("ActionKind", kind))
                using (LogContext.PushProperty("ActionName", name))
                using (LogContext.PushProperty("ActionSeverity", (int)severity))
                using (LogContext.PushProperty("ActionPayload", payload))
                {
                    switch (category)
                    {
                        case LogCategories.Information:
                            _logger.Information(name);
                            break;
                        case LogCategories.Debug:
                            _logger.Debug(name);
                            break;
                        case LogCategories.Notice:
                            _logger.Verbose(name);
                            break;
                        case LogCategories.Warning:
                            _logger.Warning(name);
                            break;
                        case LogCategories.Error:
                            _logger.Error(exception, name);
                            break;
                        case LogCategories.Fetal:
                            _logger.Fatal(name);
                            break;
                        case LogCategories.Audit:
                            _logger.Verbose(name);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
   }
}