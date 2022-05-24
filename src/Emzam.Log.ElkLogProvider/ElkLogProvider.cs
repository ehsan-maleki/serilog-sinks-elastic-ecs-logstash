using System;
using System.Collections.Generic;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Microsoft.Extensions.Configuration;
using Serilog.Context;
using Serilog.Core;

namespace Emzam.Log.ElkLogProvider
{
    public class ElkLogProvider : ILogProvider
    {
        private Logger TheLogger { get; }
        private LogApplicationModel Application { get; set; }

        internal ElkLogProvider(Logger logger, LogApplicationModel application)
        {
            TheLogger = logger;
            Application = application;
        }

        public void SetApplication(LogApplicationModel application)
            => Application = new LogApplicationModel(application);
        
        public void SetApplication(IConfiguration configuration)
        {
            Application = new LogApplicationModel
            {
                Id = configuration["Logging:Application:Id"],
                Name = configuration["Logging:Application:Name"],
                Type = (ApplicationTypes) System.Enum.Parse(typeof(ApplicationTypes), configuration["Logging:Application:Type"]),
                Version = configuration["Logging:Application:Version"],
                Server = configuration["Logging:Application:Server"],
            };
        }    
        
        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="payload">All additional data to pass to log.</param>
        public void LogInformation(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs")
        {
            Log(LogLevel.Information, category, kind, name, payload: payload, severity: Severities.Low);
        }

        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="payload">All additional data to pass to log.</param>
        public void LogDebug(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs")
        {
            Log(LogLevel.Debug, category, kind, name, payload: payload);
        }

        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="payload">All additional data to pass to log.</param>
        public void LogWarning(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs")
        {
            Log(LogLevel.Warning, category, kind, name, payload: payload, severity: Severities.High);
        }

        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="payload">All additional data to pass to log.</param>
        public void LogAudit(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs")
        {
            Log(LogLevel.Audit, category, kind, name, payload: payload);
        }

        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="payload">All additional data to pass to log.</param>
        public void LogCritical(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs")
        {
            Log(LogLevel.Notice, category, kind, name, payload: payload, severity: Severities.Critical);
        }

        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="payload">All additional data to pass to log.</param>
        public void LogFetal(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs")
        {
            Log(LogLevel.Fetal, category, kind, name, payload: payload, severity: Severities.Fetal);
        }

        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="exception">Exception object.</param>
        /// <param name="payload">All additional data to pass to log.</param>
        /// <param name="severity">Level of importance.</param>
        public void LogError(string name, Exception exception, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs",
            Severities severity = Severities.High)
        {
            Log(LogLevel.Error, category, kind, name, exception, payload, severity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level">User LogLevel enum.</param>
        /// <param name="category">
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </param>
        /// <param name="kind">
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state, 
        /// default: log, 
        /// </param>
        /// <param name="name">
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </param>
        /// <param name="exception">Exception object.</param>
        /// <param name="payload">All additional data to pass to log.</param>
        /// <param name="severity">Level of importance.</param>
        private void Log(LogLevel level, string category, string kind, string name, Exception exception = null,
            Dictionary<string, string> payload = null, Severities severity = Severities.Normal)
        {
            category = string.IsNullOrWhiteSpace(category) ? "default-logs" : category;
            kind = string.IsNullOrWhiteSpace(kind) ? "logs" : kind;
            name = string.IsNullOrWhiteSpace(name) ? "logs" : name;

            try
            {
                using (LogContext.PushProperty("ApplicationId", Application.Id))
                using (LogContext.PushProperty("ApplicationName", Application.Name))
                using (LogContext.PushProperty("ApplicationType", Application.Type))
                using (LogContext.PushProperty("ApplicationVersion", Application.Version))
                using (LogContext.PushProperty("ServerName", Application.Server))
                using (LogContext.PushProperty("ActionId", Application.Id))
                using (LogContext.PushProperty("ActionSeverity", (int) severity))
                using (LogContext.PushProperty("ActionLevel", level))
                using (LogContext.PushProperty("ActionCategory", category))
                using (LogContext.PushProperty("ActionKind", kind))
                using (LogContext.PushProperty("ActionName", name))
                using (LogContext.PushProperty("ActionPayload", payload))
                {
                    switch (level)
                    {
                        case LogLevel.Information:
                            TheLogger.Information(name);
                            break;
                        case LogLevel.Debug:
                            TheLogger.Debug(name);
                            break;
                        case LogLevel.Notice:
                            TheLogger.Verbose(name);
                            break;
                        case LogLevel.Warning:
                            TheLogger.Warning(name);
                            break;
                        case LogLevel.Error:
                            TheLogger.Error(exception, name);
                            break;
                        case LogLevel.Fetal:
                            TheLogger.Fatal(name);
                            break;
                        case LogLevel.Audit:
                            TheLogger.Verbose(name);
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