namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class HttpResponseModel
    {
        public HttpBodyModel Body { get; set; }       

        /// <summary>
        /// Total size in bytes of the request (body and headers).
        /// type: long
        /// example: 1437
        /// </summary>
        public long? Bytes { get; set; }       
       
        /// <summary>
        /// HTTP response status code.
        /// type: long
        /// example: 404
        /// </summary>
        public long? StatusCode { get; set; }       
    }
}