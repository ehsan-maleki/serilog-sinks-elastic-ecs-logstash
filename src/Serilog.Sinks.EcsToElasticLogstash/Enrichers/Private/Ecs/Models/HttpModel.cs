namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Fields related to HTTP activity. Use the url field set to store the url of the request.
    /// </summary>
    public class HttpModel
    {
        public HttpRequestModel Request { get; set; }       
       
        public HttpResponseModel Response { get; set; }       
       
        public string Version { get; set; }       
    }
}