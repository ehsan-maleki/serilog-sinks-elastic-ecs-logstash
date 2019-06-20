using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Serilog.Enrichers.Private.Ecs.Models;
using Serilog.Events;

namespace Serilog.Enrichers.Private.Ecs
{
    public class LogEventToEcsConverter
    {
        public static BaseModel ConvertToEcs(LogEvent e)
        {
            try
            {
                var request = HttpContext.Current?.Request;
                var response = HttpContext.Current?.Response;
                var error = e.Exception;
                var errors = error != null
                    ? new[] {error}
                    : HttpContext.Current?.AllErrors;
                var user = HttpContext.Current?.User;
                var thread = System.Threading.Thread.CurrentThread;

                var ecsModel = new BaseModel
                {
                    Timestamp = e.Timestamp,
                    Ecs = new EcsModel(),
                    Payload = e.Properties.ContainsKey("ActionPayload")
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        ? (e.Properties["ActionPayload"] as SequenceValue)?.Elements
                        .Select(x => x.ToString()
                            .Replace("\"", string.Empty)
                            .Replace("\"", string.Empty)
                            .Replace("[", string.Empty)
                            .Replace("]", string.Empty)
                            .Split(','))
                        .Select(value => $"{value[0].Trim()}={value[1].Trim()}")
                        .ToList()
                        : null,
                    Agent = new AgentModel
                    {
                        Id = e.Properties.ContainsKey("ApplicationId")
                            ? e.Properties["ApplicationId"].ToString()
                            : null,
                        Name = e.Properties.ContainsKey("ApplicationName")
                            ? e.Properties["ApplicationName"].ToString()
                            : null,
                        Type = e.Properties.ContainsKey("ApplicationType")
                            ? e.Properties["ApplicationType"].ToString()
                            : null,
                        Version = e.Properties.ContainsKey("ApplicationVersion")
                            ? e.Properties["ApplicationVersion"].ToString()
                            : null
                    },
                    Client = new ClientModel
                    {
                        Address = request?.UserHostAddress,
                        Ip = request?.UserHostAddress,
                        Bytes = request?.TotalBytes ?? 0,
                        User = string.IsNullOrEmpty(user?.Identity?.Name)
                            ? null
                            : new UserModel
                            {
                                Id = user?.Identity?.Name,
                                Name = user?.Identity?.Name,
                                Email = user?.Identity?.Name
                            }
                    },
                    Event = new EventModel
                    {
                        Created = DateTime.UtcNow,
                        Category = e.Properties.ContainsKey("ActionCategory")
                            ? e.Properties["ActionCategory"].ToString()
                            : null,
                        Action = e.Properties.ContainsKey("ActionName")
                            ? e.Properties["ActionName"].ToString().Replace("\"", "")
                            : null,
                        Id = e.Properties.ContainsKey("ActionId")
                            ? e.Properties["ActionId"].ToString().Replace("\"", "")
                            : null,
                        Kind = e.Properties.ContainsKey("ActionKind")
                            ? e.Properties["ActionKind"].ToString().Replace("\"", "")
                            : null,
                        Severity = e.Properties.ContainsKey("ActionSeverity")
                            ? long.Parse(e.Properties["ActionSeverity"].ToString())
                            : 0,
                        Timezone = TimeZone.CurrentTimeZone.StandardName
                    },
                    Error = errors != null && errors.Length > 0
                        ? new ErrorModel
                        {
                            Message = errors[0].Message,
                            StackTrace = CatchErrors(errors),
                            Code = errors[0].GetType().ToString()
                        }
                        : null,
                    Http = new HttpModel
                    {
                        Request = new HttpRequestModel
                        {
                            Method = request?.HttpMethod,
                            ContentEncoding = request?.ContentEncoding?.ToString(),
                            IsLocal = request?.IsLocal,
                            IsAuthenticated = request?.IsAuthenticated,
                            IsSecureConnection = request?.IsSecureConnection,
                            ContentType = request?.ContentType,
                            Headers = request?.Headers.AllKeys.Select(key => $"{key}={request?.Headers[key]}").ToList(),
                            ServerVariables = request?.ServerVariables.AllKeys.Select(key => $"{key}={request?.ServerVariables[key]}").ToList(),
                            Cookies = request?.Cookies.AllKeys.Select(key => $"{key}={request?.Cookies[key]?.Value}").ToList(),
                            Files = request?.Files?.AllKeys.ToList(),
                            Form = request?.Form?.AllKeys.Select(key => $"{key}={request?.Form[key]}").ToList(),
                            ContentLength = request?.ContentLength,
                            Bytes = request?.TotalBytes ?? 0,
                            Body = new HttpBodyModel
                            {
                                Bytes = request?.TotalBytes ?? 0,
                                Content = request?.InputStream.ToString()
                            },
                            Referrer = request?.UrlReferrer?.ToString()
                        },
                        Response = new HttpResponseModel
                        {
                            Bytes = 0, // response?.OutputStream.Length ?? 0,
                            StatusCode = response?.StatusCode ?? 0,
                            Body = new HttpBodyModel
                            {
                                Bytes = 0, //response?.OutputStream.Length ?? 0,
                                Content = response?.OutputStream.ToString()
                            }
                        }
                    },
                    Process = new ProcessModel
                    {
                        Title = thread.Name,
                        Name = thread.Name,
                        Executable = thread.ExecutionContext.GetType().ToString(),
                        Thread = new ThreadModel
                        {
                            Id = thread.ManagedThreadId,
                        }
                    },
                    Server = new ServerModel
                    {
                        Domain = request?.Url?.Authority,
                        User = e.Properties.ContainsKey("EnvironmentUserName")
                            ? e.Properties["EnvironmentUserName"].ToString()
                            : null,
                        Address = e.Properties.ContainsKey("Host")
                            ? e.Properties["Host"].ToString()
                            : null,
                        Ip = e.Properties.ContainsKey("Host")
                            ? e.Properties["Host"].ToString()
                            : null,
                    },
                    Log = new LogModel
                    {
                        Level = $"{e.Level}"
                    },
                    Url = new UrlModel
                    {
                        Original = request?.RawUrl,
                        Full = request?.Url.ToString(),
                        Path = request?.Path,
                        Scheme = request?.Url.Scheme,
                        Query = request?.Url?.Query,
                        Domain = request?.Url?.Authority,
                        Username = request?.LogonUserIdentity?.Name,
                        Port = request?.Url?.Port ?? 0
                    },
                    User = string.IsNullOrEmpty(user?.Identity?.Name)
                        ? null
                        : new UserModel
                        {
                            Id = user?.Identity?.Name,
                            Name = user?.Identity?.Name,
                            Email = user?.Identity?.Name
                        },
                    UserAgent = new UserAgentModel
                    {
                        IsMobileDevice = request?.Browser?.IsMobileDevice,
                        Device = new DeviceModel
                        {
                            Name = request?.Browser?.MobileDeviceModel,
                            Manufacturer = request?.Browser?.MobileDeviceManufacturer
                        },
                        Name = request?.UserAgent,
                        Original = request?.UserAgent,
                        Platform = request?.Browser?.Platform,
                        ScreenPixelsWidth = request?.Browser?.ScreenPixelsWidth,
                        ScreenPixelsHeight = request?.Browser?.ScreenPixelsHeight,
                        IsCrawler = request?.Browser?.Crawler,
                        Type = request?.Browser?.Type,
                        Version = request?.Browser?.Version,
                    }
                };

                return ecsModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string CatchErrors(IReadOnlyCollection<Exception> errors)
        {
            if (errors == null || errors.Count <= 0)
                return string.Empty;

            var i = 1;
            var fullText = new StringWriter();
            foreach (var error in errors)
            {
                var frame = new StackTrace(error, true).GetFrame(0);

                fullText.WriteLine($"Exception {i++:D2} ===================================");
                fullText.WriteLine($"Type: {error.GetType()}");
                fullText.WriteLine($"Source: {error.TargetSite?.DeclaringType?.AssemblyQualifiedName}");
                fullText.WriteLine($"Message: {error.Message}");
                fullText.WriteLine($"Trace: {error.StackTrace}");
                fullText.WriteLine($"Location: {frame.GetFileName()}");
                fullText.WriteLine($"Method: {frame.GetMethod()} ({frame.GetFileLineNumber()}, {frame.GetFileColumnNumber()})");

                var exception = error.InnerException;
                while (exception != null)
                {
                    frame = new StackTrace(exception, true).GetFrame(0);
                    fullText.WriteLine($"\tException {i:D2} inner --------------------------");
                    fullText.WriteLine($"\tType: {exception.GetType()}");
                    fullText.WriteLine($"\tSource: {exception.TargetSite?.DeclaringType?.AssemblyQualifiedName}");
                    fullText.WriteLine($"\tMessage: {exception.Message}");
                    fullText.WriteLine($"\tLocation: {frame.GetFileName()}");
                    fullText.WriteLine($"\tMethod: {frame.GetMethod()} ({frame.GetFileLineNumber()}, {frame.GetFileColumnNumber()})");

                    exception = exception.InnerException;
                }
            }

            return fullText.ToString();
        }
    }
}