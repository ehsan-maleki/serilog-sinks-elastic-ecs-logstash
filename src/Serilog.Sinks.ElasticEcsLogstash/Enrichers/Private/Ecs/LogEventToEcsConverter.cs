using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog.Enrichers.Private.Ecs.Models;
using Serilog.Events;
using UAParser;

// ReSharper disable ClassNeverInstantiated.Global

namespace Serilog.Enrichers.Private.Ecs
{
    public class LogEventToEcsConverter
    {
        public static BaseModel ConvertToEcs(HttpContext context, LogEvent e)
        {
            // ============================================================================
            // PAYLOAD
            var payload = new List<string>();
            var pairs = (e.Properties["ActionPayload"] as DictionaryValue)?.Elements;
            if (pairs != null)
                payload.AddRange(from key in pairs.Keys
                    let k = key.ToString().Trim(' ', '"')
                    let v = pairs[key].ToString().Trim(' ', '"')
                    select $"{k}={v}"
                );

            var thread = System.Threading.Thread.CurrentThread;
            var serverName = e.Properties.ContainsKey("ServerName")
                ? e.Properties["ServerName"].ToString().Trim(' ', '"')
                : "localhost";

            // host information
            var host = new IPHostEntry
            {
                HostName = serverName,
                Aliases = new string[] { },
                AddressList = new[] {new IPAddress(new byte[] {127, 0, 0, 1})}
            };

            try
            {
                host = Dns.GetHostEntry(serverName);
            }
            catch
            {
                // Ignore
            }

            var hostNames = host.HostName +
                            (host.Aliases.Any() ? "," + string.Join(",", host.Aliases) : "");
            var hostIps = host.AddressList.Any()
                ? string.Join(",", host.AddressList.Select(x => x.ToString()))
                : "";

            var ecsModel = new BaseModel
            {
                Timestamp = e.Timestamp,
                Ecs = new EcsModel(),

                #region -- PAYLOAD --

                Payload = payload,

                #endregion

                #region -- AGENT --

                Agent = new AgentModel
                {
                    Id = e.Properties.ContainsKey("ApplicationId")
                        ? e.Properties["ApplicationId"].ToString().Trim('"', ' ')
                        : null,
                    Name = e.Properties.ContainsKey("ApplicationName")
                        ? e.Properties["ApplicationName"].ToString().Trim('"', ' ')
                        : null,
                    Type = e.Properties.ContainsKey("ApplicationType")
                        ? e.Properties["ApplicationType"].ToString().Trim('"', ' ')
                        : null,
                    Version = e.Properties.ContainsKey("ApplicationVersion")
                        ? e.Properties["ApplicationVersion"].ToString().Trim('"', ' ')
                        : null
                },

                #endregion

                #region -- EVENT --

                Event = new EventModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Created = DateTime.UtcNow,
                    Level = e.Properties.ContainsKey("ActionLevel")
                        ? e.Properties["ActionLevel"].ToString().Replace("\"", "")
                        : null,
                    Category = e.Properties.ContainsKey("ActionCategory")
                        ? e.Properties["ActionCategory"].ToString().Replace("\"", "")
                        : null,
                    Kind = e.Properties.ContainsKey("ActionKind")
                        ? e.Properties["ActionKind"].ToString().Replace("\"", "")
                        : null,
                    Action = e.Properties.ContainsKey("ActionName")
                        ? e.Properties["ActionName"].ToString().Replace("\"", "")
                        : null,
                    Severity = e.Properties.ContainsKey("ActionSeverity")
                        ? long.Parse(e.Properties["ActionSeverity"].ToString())
                        : 0,
                    Timezone = TimeZoneInfo.Local.DisplayName
                },

                #endregion

                #region -- ERROR --

                Error = e.Exception != null
                    ? new ErrorModel
                    {
                        Message = e.Exception.Message,
                        StackTrace = CatchErrors(e.Exception),
                        Code = e.Exception.GetType().ToString()
                    }
                    : null,

                #endregion

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
                    Domain = hostNames,
                    Address = hostNames
                },

                #endregion

                Log = new LogModel
                {
                    Level = $"{e.Level}"
                }
            };

            if (context == null)
                return ecsModel;

            try
            {
                var traceIdentifier = context.TraceIdentifier ?? ecsModel.Event.Id;
                var connection = context.Connection;
                // Gets or sets a unique identifier to represent this connection.
                var localIp = connection?.LocalIpAddress ?? IPAddress.Loopback;
                var localPort = connection?.LocalPort ?? 80;

                // Gets or sets a key/value collection that can be used to share data within the scope of this request.
                var items = context.Items ?? new Dictionary<object, object>();

                // ============================================================================
                // Gets the HttpRequest object for this request.
                var request = context.Request;
                var https = request?.IsHttps ?? false;
                var method = request?.Method ?? "None";
                var scheme = request?.Scheme ?? "None";

                var displayUrl = request?.GetDisplayUrl() ?? string.Empty;
                var displayUrlDomain = string.Empty;
                if (!string.IsNullOrWhiteSpace(displayUrl))
                {
                    var index1 = displayUrl.IndexOf("//", StringComparison.Ordinal) + 2;
                    displayUrlDomain = displayUrl.Substring(index1);
                    var index2 = displayUrlDomain.IndexOf(":", StringComparison.Ordinal) > 0
                        ? displayUrlDomain.IndexOf(":", StringComparison.Ordinal)
                        : displayUrlDomain.IndexOf("/", StringComparison.Ordinal) > 0
                            ? displayUrlDomain.IndexOf("/", StringComparison.Ordinal)
                            : displayUrlDomain.Length - 1;
                    displayUrlDomain = displayUrlDomain.Substring(0, index2);
                }

                var encodedUrl = request?.GetEncodedUrl() ?? string.Empty;
                var requestPath = request?.Path ?? string.Empty;
                var hasQueryString = request?.QueryString.HasValue ?? false;
                var queryStringValue = request?.QueryString.Value ?? string.Empty;

                var headers = context.Request.Headers;
                var headerKeys = headers?.Keys ?? new List<string>();
                var refererHeader = headers?.ContainsKey("Referer") ?? false ? headers["Referer"].ToString() : string.Empty;

                var userAgentHeader = headers?.ContainsKey("User-Agent") ?? false ? headers["User-Agent"].ToString() : string.Empty;
                var ua = Parser.GetDefault().Parse(userAgentHeader);
                var isMobileDevice = ua?.Device?.Family?.ToLower() == "ios" ||
                                     ua?.Device?.Family?.ToLower() == "android";

                // Client information
                var clientIp = headerKeys.Contains("XX_REAL_IP")
                    ? headers["XX_REAL_IP"].First()
                    : headerKeys.Contains("X_REAL_IP")
                        ? headers["X_REAL_IP"].First()
                        : connection?.RemoteIpAddress.ToString() ?? string.Empty;
                var clientPort = connection?.RemotePort ?? 0;

                var requestContentLength = request?.ContentLength ?? headers?.ContentLength ?? 0;
                var requestContentType = request?.ContentType ?? "Unknown";
                var hasFormContentType = request?.HasFormContentType ?? false; // Checks the Content-Type header for form types.

                var hasForm = hasFormContentType && requestContentType != null && requestContentLength > 0;

                List<string> formItems = null;
                List<string> formFiles = null;
                if (hasForm)
                {
                    try
                    {
                        var requestForm = request.Form; // Represents the parsed form values sent with the HttpRequest.
                        var formCount = requestForm.Count; // Gets the number of elements contained in the IFormCollection.
                        var formKeys = requestForm.Keys; // Gets an ICollection<T> containing the keys of the IFormCollection.
                        formItems = formCount > 0 // The file collection sent with the request.
                            ? formKeys.Select(x => $"{x}={requestForm[x]}").ToList()
                            : null;
                        var filesCount = requestForm.Files?.Count ?? 0;
                        formFiles = filesCount > 0 // The file collection sent with the request.
                            ? requestForm.Files!.Select(x => $"{x.Name}={x.Length}").ToList()
                            : null;
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                var cookies = request?.Cookies; // Represents the HttpRequest cookie collection.
                var cookiesKeys = cookies?.Keys ?? new List<string>(); // Gets an ICollection<T> containing the keys of the IRequestCookieCollection.

                // ============================================================================
                // Gets the HttpResponse object for this request.
                var response = context.Response;
                var responseStatusCode = response?.StatusCode ?? 0;
                var responseContentLength = response?.ContentLength ?? 0;

                // ============================================================================
                // Gets or sets the user for this request.
                var principal = context.User;
                var isAuthenticated = principal?.Identity.IsAuthenticated ?? false;
                var userName = principal?.Identity.Name ?? "Anonymous";

                var claims = principal?.Claims.Any() ?? false
                    ? string.Join(",", principal.Claims.Select(x => $"{x.Type}={x.Value}"))
                    : null;

                ecsModel.Event.Id = traceIdentifier;

                #region -- CLIENT --

                ecsModel.Client = new ClientModel
                {
                    Address = clientIp,
                    Ip = clientIp
                };

                #endregion

                ecsModel.Http = new HttpModel
                {
                    Items = items?.Select(key => $"{key}={items[key]}").ToList(),

                    #region -- REQUEST --

                    Request = new HttpRequestModel
                    {
                        Method = method,
                        IsLocal = clientIp.IndexOf("127.0.0.1", StringComparison.Ordinal) >= 0,
                        IsAuthenticated = isAuthenticated,
                        IsSecureConnection = https,
                        ContentType = requestContentType,
                        ContentLength = requestContentLength,
                        Bytes = requestContentLength,

                        Headers = headerKeys?.Select(key => $"{key}={headers[key]}").ToList(),
                        Cookies = cookiesKeys?.Select(key => $"{key}={cookies[key]}").ToList(),
                        Form = formItems,
                        Files = formFiles,

                        Body = new HttpBodyModel
                        {
                            Bytes = requestContentLength
                        },
                        Referrer = refererHeader
                    },

                    #endregion

                    #region -- RESPONSE --

                    Response = new HttpResponseModel
                    {
                        Bytes = responseContentLength,
                        StatusCode = responseStatusCode,
                        Body = new HttpBodyModel
                        {
                            Bytes = responseContentLength
                        }
                    }

                    #endregion
                };

                ecsModel.Server.Ip = localIp + "," + hostIps;
                ecsModel.Server.Port = localPort;

                #region -- URL --

                ecsModel.Url = new UrlModel
                {
                    Original = displayUrl,
                    Full = encodedUrl,
                    Path = requestPath,
                    Scheme = scheme,
                    Query = hasQueryString ? queryStringValue : null,
                    Username = userName,
                    Domain = displayUrlDomain,
                    Port = clientPort
                };

                #endregion

                #region -- USER --

                ecsModel.User = !isAuthenticated
                    ? null
                    : new UserModel
                    {
                        Id = userName,
                        Name = userName,
                        Hash = claims
                    };

                #endregion

                #region -- USER AGENT --

                ecsModel.UserAgent = new UserAgentModel
                {
                    IsMobileDevice = isMobileDevice,
                    Device = new DeviceModel
                    {
                        Name = ua.Device.Family + " " + ua.Device.Model,
                        Manufacturer = ua.Device.Brand
                    },
                    Name = ua.UA.Family,
                    Original = ua.String,
                    Platform = ua.UA.Family,
                    IsCrawler = ua.Device.IsSpider,
                    Type = ua.UA.Family,
                    Version = ua.UA.Major + "." + ua.UA.Minor,
                };

                #endregion
            }
            catch (Exception ex)
            {
                ecsModel.Error = new ErrorModel
                {
                    Message = ex.Message,
                    StackTrace = CatchErrors(ex),
                    Code = ex.GetType().ToString()
                };
            }

            return ecsModel;
        }

        private static string CatchErrors(Exception error)
        {
            if (error == null)
                return string.Empty;

            var i = 1;
            var fullText = new StringWriter();
            var frame = new StackTrace(error, true).GetFrame(0);

            fullText.WriteLine("<div style='padding 10px; margin 0 0 30px 0;'>");
            fullText.WriteLine($"<h3>Exception {i:D2} ===================================</h3>");
            fullText.WriteLine($"Type: {error.GetType()}");
            fullText.WriteLine($"<br />Source: {error.TargetSite?.DeclaringType?.AssemblyQualifiedName}");
            fullText.WriteLine($"<br />Message: {error.Message}");
            fullText.WriteLine($"<br />Trace: {error.StackTrace}");
            fullText.WriteLine($"<br />Location: {frame.GetFileName()}");
            fullText.WriteLine($"<br />Method: {frame.GetMethod()} ({frame.GetFileLineNumber()}, {frame.GetFileColumnNumber()})");
            fullText.WriteLine("</div>");

            var exception = error.InnerException;
            while (exception != null)
            {
                frame = new StackTrace(exception, true).GetFrame(0);
                fullText.WriteLine("<div style='padding 10px; margin 0 0 30px 30px;'>");
                fullText.WriteLine($"<h4>Exception {i++:D2} inner --------------------------</h4>");
                fullText.WriteLine($"Type: {exception.GetType()}");
                fullText.WriteLine($"<br />Source: {exception.TargetSite?.DeclaringType?.AssemblyQualifiedName}");
                fullText.WriteLine($"<br />Message: {exception.Message}");
                fullText.WriteLine($"<br />Location: {frame.GetFileName()}");
                fullText.WriteLine($"<br />Method: {frame.GetMethod()} ({frame.GetFileLineNumber()}, {frame.GetFileColumnNumber()})");
                fullText.WriteLine("</div>");

                exception = exception.InnerException;
            }

            return fullText.ToString();
        }
    }
}