using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Serilog.Enrichers.Private.Ecs.Models;
using Serilog.Events;

namespace Serilog.Enrichers.Private.Ecs
{
    public class LogEventToEcsConverter
    {
        public static BaseModel ConvertToEcs(HttpContext context, LogEvent e)
        {
            try
            {
                var request = context.Request;
                var headers = context.Request.Headers;
                var response = context.Response;
                var connection = context.Connection;
                var error = e.Exception;
                var user = context.User;
                var thread = System.Threading.Thread.CurrentThread;

                var ecsModel = new BaseModel
                {
                    Timestamp = e.Timestamp,
                    Ecs = new EcsModel(),

                    #region -- PAYLOAD --

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

                    #endregion

                    #region -- AGENT --

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

                    #endregion

                    #region -- CLIENT --

                    Client = new ClientModel
                    {
                        Address = connection?.RemoteIpAddress + ":" +
                                  connection?.RemotePort,
                        Ip = headers.Keys.Contains("X_REAL_IP")
                        ? headers["X_REAL_IP"].FirstOrDefault()
                        : connection?.RemoteIpAddress.ToString(),
                        Bytes = request?.ContentLength ?? 0,
                        User = string.IsNullOrEmpty(user?.Identity?.Name)
                            ? null
                            : new UserModel
                            {
                                Id = user.Identity?.Name,
                                Name = user.Identity?.Name,
                                Email = user.Identity?.Name
                            }
                    },

                    #endregion

                    #region -- EVENT --

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
                        Timezone = TimeZoneInfo.Local.DisplayName
                    },

                    #endregion

                    #region -- ERROR --

                    Error = error != null
                        ? new ErrorModel
                        {
                            Message = error.Message,
                            StackTrace = CatchErrors(error),
                            Code = error.GetType().ToString()
                        }
                        : null,

                    #endregion

                    Http = new HttpModel
                    {
                        #region -- REQUEST --

                        Request = new HttpRequestModel
                        {
                            Method = request?.Method,
                            // ContentEncoding = request?.ContentEncoding?.ToString(),
                            IsLocal = false,
                            IsAuthenticated = !string.IsNullOrEmpty(context.User?.Identity?.Name),
                            IsSecureConnection = request?.IsHttps,
                            ContentType = request?.ContentType,
                            Headers = request?.Headers.Keys.Select(key => $"{key}={request?.Headers[key]}").ToList(),
                            // ServerVariables = request?.ServerVariables.AllKeys.Select(key => $"{key}={request?.ServerVariables[key]}").ToList(),
                            Cookies = request?.Cookies.Keys.Select(key => $"{key}={request?.Cookies[key]}").ToList(),
                            // Files = request?.Files?.AllKeys.ToList(),
                            ContentLength = request?.ContentLength,
                            Form = string.IsNullOrEmpty(request?.ContentType) || (request?.ContentLength ?? 0) == 0
                                ? null
                                : request?.Form?.Keys.Select(key => $"{key}={request?.Form[key]}").ToList(),
                            Bytes = request?.ContentLength ?? 0,
                            Body = new HttpBodyModel
                            {
                                Bytes = request?.ContentLength ?? 0,
                                Content = (request?.ContentLength ?? 0) > 0 && (request?.Body.CanRead ?? false) ? request?.Body.ToString() : null
                            },
                            Referrer = request?.Headers["Referer"].ToString()
                        },

                        #endregion

                        #region -- RESPONSE --

                        Response = new HttpResponseModel
                        {
                            // Bytes = 0, // response?.OutputStream.Length ?? 0,
                            StatusCode = response?.StatusCode ?? 0 /*,
                            Body = new HttpBodyModel
                            {
                                Bytes = 0, //response?.OutputStream.Length ?? 0,
                                Content = response?.Body.ToString()
                            }*/
                        }

                        #endregion
                    },

                    #region -- PROCESS --

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

                    #endregion

                    #region -- SERVER --

                    Server = new ServerModel
                    {
                        Domain = request.Host.Host,
                        User = e.Properties.ContainsKey("EnvironmentUserName")
                            ? e.Properties["EnvironmentUserName"].ToString()
                            : null,
                        Address = e.Properties.ContainsKey("Host")
                            ? e.Properties["Host"].ToString()
                            : null,
                        Ip = request.Host.Host
                    },

                    #endregion

                    Log = new LogModel
                    {
                        Level = $"{e.Level}"
                    },

                    #region -- URL --

                    Url = new UrlModel
                    {
                        Original = $"{request?.Host}{request?.QueryString}",
                        Full = $"{request?.Host}{request?.QueryString}",
                        Path = request?.Path,
                        Scheme = request?.Scheme,
                        Query = request?.QueryString.ToString(),
                        Domain = request?.Host.Host,
                        Username = user?.Identity?.Name,
                        Port = request?.Host.Port ?? 0
                    },

                    #endregion

                    #region -- USER --

                    User = string.IsNullOrEmpty(user?.Identity?.Name)
                        ? null
                        : new UserModel
                        {
                            Id = user?.Identity?.Name,
                            Name = user?.Identity?.Name,
                            Email = user?.Identity?.Name
                        } //,

                    #endregion

                    #region -- USER AGENT --

                    /*UserAgent = new UserAgentModel
                    {
                        IsMobileDevice = request?.?.IsMobileDevice,
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
                    }*/

                    #endregion
                };

                return ecsModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static string CatchErrors(Exception error)
        {
            if (error == null)
                return string.Empty;

            var i = 1;
            var fullText = new StringWriter();
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

            return fullText.ToString();
        }
    }
}