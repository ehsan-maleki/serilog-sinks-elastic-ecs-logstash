using Emzam.Log.ElkLogProvider.Enum;
using Emzam.Log.ElkLogProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Emzam.Log.ElkLogProvider;

public class ElkLogConfig
{
    private LogEventLevel LogLevel { get; set; }
    private LogApplicationModel Application { get; set; }

    #region << DEBUG >>

    public const string DefaultTemplate = "[{Timestamp:ddd yyyy-MM-dd hh:mm:ss tt} {Level}] {Properties}{NewLine}{NewLine}";
    private bool WriteToDebug { get; set; }
    private string DebugTemplate { get; set; }

    #endregion

    #region << CONSOLE >>

    private bool WriteToConsole { get; set; }
    private string ConsoleTemplate { get; set; }

    #endregion

    #region << LOGSTASH >>

    private const string DefaultLogstashUrl = "http://localhost:8080";
    private bool WriteToLogstash { get; set; }
    private IHttpContextAccessor Accessor { get; set; }
    private string LogstashUrl { get; set; }

    #endregion

    public ElkLogConfig()
    {
        LogLevel = LogEventLevel.Verbose;
        Application = new LogApplicationModel();

        WriteToDebug = true;
        DebugTemplate = DefaultTemplate;

        WriteToConsole = false;
        ConsoleTemplate = DefaultTemplate;

        WriteToLogstash = false;
        LogstashUrl = DefaultLogstashUrl;
    }

    public ElkLogConfig SetApplication(LogApplicationModel application = null)
    {
        Application = new LogApplicationModel(application);
        return this;
    }

    public ElkLogConfig SetApplication(IConfiguration configuration)
    {
        Application = new LogApplicationModel
        {
            Id = configuration["Logging:Application:Id"],
            Name = configuration["Logging:Application:Name"],
            Type = (ApplicationTypes) System.Enum.Parse(typeof(ApplicationTypes), configuration["Logging:Application:Type"]),
            Version = configuration["Logging:Application:Version"],
            Server = configuration["Logging:Application:Server"],
        };
        return this;
    }

    public ElkLogConfig SetAccessor(IHttpContextAccessor accessor = default)
    {
        Accessor = accessor ?? Accessor;
        return this;
    }

    public ElkLogConfig UseDebug(string template = DefaultTemplate)
    {
        WriteToDebug = true;
        DebugTemplate = template ?? DefaultTemplate;
        return this;
    }

    public ElkLogConfig UseConsole(string template = DefaultTemplate)
    {
        WriteToConsole = true;
        ConsoleTemplate = template ?? DefaultTemplate;
        return this;
    }

    public ElkLogConfig UseLogstash(string url = DefaultLogstashUrl)
    {
        WriteToLogstash = true;
        LogstashUrl = url ?? DefaultLogstashUrl;
        return this;
    }

    public ElkLogProvider CreateLogger(IConfiguration configuration, IHttpContextAccessor accessor = default)
    {
        WriteToDebug = false;
        WriteToConsole = false;
        WriteToLogstash = false;

        Accessor = accessor;
        LogLevel = (LogEventLevel) System.Enum.Parse(typeof(LogEventLevel), configuration["Logging:Level"]);
        SetApplication(configuration);

        var config = new LoggerConfiguration()
            .MinimumLevel.Is(LogLevel)
            .Enrich.FromLogContext()
            .Enrich.WithEcs(Accessor);


        if (configuration["Logging:Writers:Debug:Enabled"]?.ToLower() == "true")
        {
            WriteToDebug = true;
            DebugTemplate = configuration["Logging:Writers:Debug:Template"] ?? DefaultTemplate;
            config.WriteTo.Debug(LogLevel, DebugTemplate);
        }

        if (configuration["Logging:Writers:Console:Enabled"]?.ToLower() == "true")
        {
            WriteToConsole = true;
            ConsoleTemplate = configuration["Logging:Writers:Console:Template"] ?? DefaultTemplate;
            config.WriteTo.Console(LogLevel, ConsoleTemplate);
        }

        if (configuration["Logging:Writers:Logstash:Enabled"]?.ToLower() == "true")
        {
            WriteToLogstash = true;
            LogstashUrl = configuration["Logging:Writers:Logstash:Url"] ?? DefaultLogstashUrl;
            config.WriteTo.ElasticEcsLogstash(
                restrictedToMinimumLevel: LogLevel,
                requestUri: LogstashUrl ?? DefaultLogstashUrl);
        }

        return new ElkLogProvider(config.CreateLogger(), Application);
    }

    public ElkLogProvider CreateLogger()
    {
        var config = new LoggerConfiguration()
            .MinimumLevel.Is(LogLevel)
            .Enrich.FromLogContext()
            .Enrich.WithEcs(Accessor);
        
        if (WriteToDebug)
            config.WriteTo.Debug(
                restrictedToMinimumLevel: LogLevel,
                outputTemplate: DebugTemplate ?? DefaultTemplate);

        if (WriteToConsole)
            config.WriteTo.Console(
                restrictedToMinimumLevel: LogLevel,
                outputTemplate: ConsoleTemplate ?? DefaultTemplate);

        if (WriteToLogstash)
        {
            config.WriteTo.ElasticEcsLogstash(
                restrictedToMinimumLevel: LogLevel,
                requestUri: LogstashUrl ?? DefaultLogstashUrl);
            config.WriteTo.DurableElasticEcsLogstashUsingFileSizeRolledBuffers(
                restrictedToMinimumLevel: LogLevel,
                requestUri: LogstashUrl ?? DefaultLogstashUrl);
        }

        return new ElkLogProvider(config.CreateLogger(), Application);
    }
}