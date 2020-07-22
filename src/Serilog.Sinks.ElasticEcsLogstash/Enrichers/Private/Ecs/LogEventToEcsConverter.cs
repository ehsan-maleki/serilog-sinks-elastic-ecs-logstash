using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog.Enrichers.Private.Ecs.Models;
using Serilog.Events;
using UAParser;

namespace Serilog.Enrichers.Private.Ecs
{
    public class LogEventToEcsConverter
    {
        public static BaseModel ConvertToEcs(HttpContext context, LogEvent e)
        {
            try
            {
                var traceIdentifier = context.TraceIdentifier;
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

                var connection = context.Connection;
                var error = e.Exception;

                // Gets or sets a unique identifier to represent this connection.
                var localIp = connection.LocalIpAddress;
                var localPort = connection.LocalPort;

                // Gets or sets a key/value collection that can be used to share data within the scope of this request.
                var items = context.Items;

                // ============================================================================
                // Gets the HttpRequest object for this request.
                var request = context.Request;
                var https = request.IsHttps;
                var method = request.Method;
                var scheme = request.Scheme;
                var requestBody = request.Body;
                var canReadBody = false;
                try
                {
                    canReadBody = (requestBody?.CanRead ?? false) && (requestBody?.Length ?? 0) > 0;
                }
                catch
                {
                    canReadBody = false;
                }

                var displayUrl = request.GetDisplayUrl();
                var index1 = displayUrl.IndexOf("//", StringComparison.Ordinal) + 2;
                var displayUrlDomain = displayUrl.Substring(index1);
                var index2 = displayUrlDomain.IndexOf(":", StringComparison.Ordinal) > 0
                    ? displayUrlDomain.IndexOf(":", StringComparison.Ordinal)
                    : displayUrlDomain.IndexOf("/", StringComparison.Ordinal) > 0
                        ? displayUrlDomain.IndexOf("/", StringComparison.Ordinal)
                        : displayUrlDomain.Length - 1;
                displayUrlDomain = displayUrlDomain.Substring(0, index2);
                var encodedUrl = request.GetEncodedUrl();

                var requestPath = request.Path;
                var hasQueryString = request.QueryString.HasValue;
                var queryStringValue = request.QueryString.Value;

                var headers = context.Request.Headers;
                var headerKeys = headers.Keys;
                var refererHeader = headers["Referer"];
                // =Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36

                var userAgentHeader = headers["User-Agent"].ToString();
                var uaParser = Parser.GetDefault();
                var ua = uaParser.Parse(userAgentHeader);
                var isMobileDevice = ua.Device.Family.ToLower() == "ios" ||
                                     ua.Device.Family.ToLower() == "android";

                // Client information
                var clientIp = headerKeys.Contains("XX_REAL_IP")
                    ? headers["XX_REAL_IP"].First()
                    : headerKeys.Contains("X_REAL_IP")
                        ? headers["X_REAL_IP"].First()
                        : connection.RemoteIpAddress.ToString();
                var clientPort = connection.RemotePort;

                var requestContentLength = request.ContentLength ?? headers.ContentLength;
                var requestContentType = request.ContentType;
                var hasFormContentType = request.HasFormContentType; // Checks the Content-Type header for form types.

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

                var cookies = request.Cookies; // Represents the HttpRequest cookie collection.
                var cookiesCount = cookies.Count; // Gets the number of elements contained in the IRequestCookieCollection.
                var cookiesKeys = cookies.Keys; // Gets an ICollection<T> containing the keys of the IRequestCookieCollection.

                // ============================================================================
                // Gets the HttpResponse object for this request.
                var response = context.Response;
                var responseHasStarted = response.HasStarted;
                var responseStatusCode = response.StatusCode;
                var responseContentLength = response.ContentLength;
                var responseContentType = response.ContentType;
                var responseBody = response.Body;
                var responseBodyCanRead = (responseBody?.CanRead ?? false) && (response?.ContentLength ?? 0) > 0;
                var responseHeaders = response.Headers;
                var responseCookies = response.Cookies;

                // ============================================================================
                // Gets or sets the object used to manage user session data for this request.
                try
                {
                    var session = context.Session;
                    var sessionIsAvailable = session.IsAvailable;
                    var sessionId = session.Id;
                    var sessionKeys = session.Keys;
                }
                catch
                {
                    // ignore
                }


                // ============================================================================
                // Gets or sets the user for this request.
                var principal = context.User;
                var isAuthenticated = principal.Identity.IsAuthenticated;
                var userName = principal.Identity.Name;
                var authenticationType = principal.Identity.AuthenticationType;

                var claims = principal.Claims.Any()
                    ? string.Join(",", principal.Claims.Select(x => $"{x.Type}={x.Value}"))
                    : null;

                var payload = new List<string>();
                var pairs = (e.Properties["ActionPayload"] as DictionaryValue)?.Elements;
                if (pairs != null)
                    foreach (var key in pairs.Keys)
                    {
                        var k = key.ToString().Trim(' ', '"');
                        var v = pairs[key].ToString().Trim(' ', '"');

                        payload.Add($"{k}={v}");
                    }

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

                    #region -- CLIENT --

                    Client = new ClientModel
                    {
                        Address = clientIp,
                        Ip = clientIp
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
                        Id = traceIdentifier,
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
                            Method = method,
                            IsLocal = clientIp.IndexOf("127.0.0.1", StringComparison.Ordinal) >= 0,
                            IsAuthenticated = isAuthenticated,
                            IsSecureConnection = https,
                            ContentType = requestContentType,
                            ContentLength = requestContentLength,
                            Bytes = requestContentLength,

                            Headers = headerKeys.Select(key => $"{key}={headers[key]}").ToList(),
                            Cookies = cookiesKeys.Select(key => $"{key}={cookies[key]}").ToList(),
                            Form = formItems,
                            Files = formFiles,

                            Body = new HttpBodyModel
                            {
                                Bytes = requestContentLength,
                                Content = canReadBody ? requestBody.ToString() : null
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
                                Bytes = responseContentLength,
                                Content = responseBodyCanRead ? responseBody.ToString() : null
                            }
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
                        Domain = hostNames,
                        Address = hostNames,
                        Ip = localIp + "," + hostIps,
                        Port = localPort
                    },

                    #endregion

                    Log = new LogModel
                    {
                        Level = $"{e.Level}"
                    },

                    #region -- URL --

                    Url = new UrlModel
                    {
                        Original = displayUrl,
                        Full = encodedUrl,
                        Path = requestPath,
                        Scheme = scheme,
                        Query = hasQueryString ? queryStringValue : null,
                        Username = userName,
                        Domain = displayUrlDomain,
                        Port = clientPort
                    },

                    #endregion

                    #region -- USER --

                    User = !isAuthenticated
                        ? null
                        : new UserModel
                        {
                            Id = userName,
                            Name = userName,
                            Hash = claims
                        },

                    #endregion

                    #region -- USER AGENT --

                    UserAgent = new UserAgentModel
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
                    }

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