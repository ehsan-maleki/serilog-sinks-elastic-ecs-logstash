using System;
using System.Collections.Generic;
using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Microsoft.Extensions.Configuration;
using Serilog.Core;

namespace Emzam.Log.ElkLogProvider
{
    public interface ILogProvider
    {
        void SetApplication(LogApplicationModel application);
        void SetApplication(IConfiguration configuration);
        
        void LogInformation(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs");
        void LogDebug(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs");
        void LogWarning(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs");
        void LogAudit(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs");
        void LogCritical(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs");
        void LogFetal(string name, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs");

        void LogError(string name, Exception exception, Dictionary<string, string> payload = null, string kind = "log", string category = "default-logs",
            Severities severity = Severities.High);
    }
}