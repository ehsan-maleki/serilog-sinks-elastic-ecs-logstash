using System;
using System.Collections.Generic;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Serilog.Core;

namespace Emzam.Log.ElkLogProvider
{
    public interface ILogProvider
    {
        LogApplicationModel _application { get; }
        Logger _logger { get; }

        void ChangeApplication(LogApplicationModel application);

        void LogInformation(string category, string name, List<KeyValuePair<string, string>> payload);
        void LogDebug(string name, List<KeyValuePair<string, string>> payload, string category = "Default Logs");
        void LogWarning(string name, List<KeyValuePair<string, string>> payload, string category = "Default Logs");
        void LogAudit(string name, List<KeyValuePair<string, string>> payload, string category = "Default Logs");
        void LogCritical(string name, List<KeyValuePair<string, string>> payload, string category = "Default Logs");
        void LogFetal(string name, List<KeyValuePair<string, string>> payload, string category = "Default Logs");
        void LogError(string name, Exception exception, List<KeyValuePair<string, string>> payload, string category = "Default Logs",
            Severities severity = Severities.High);
    }
}