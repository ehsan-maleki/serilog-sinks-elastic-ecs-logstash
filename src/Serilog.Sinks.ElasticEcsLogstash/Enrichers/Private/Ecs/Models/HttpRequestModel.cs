using System.Collections.Generic;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class HttpRequestModel
    {
        /// <summary>
        /// HTTP request method.
        /// The field value must be normalized to lowercase for querying. See the documentation section "Implementing ECS".
        /// type: keyword
        /// example: get, post, put
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Content encoding of request
        /// type: keyword
        /// example: UTF-8
        /// </summary>
        public string ContentEncoding { get; set; }

        /// <summary>
        /// This is a local (127.0.0.1) request?
        /// type: boolean
        /// example: true
        /// </summary>
        public bool? IsLocal { get; set; }

        /// <summary>
        /// User of this request is authenticated?
        /// type: boolean
        /// example: false
        /// </summary>
        public bool? IsAuthenticated { get; set; }

        /// <summary>
        /// Current scheme of request is https?
        /// type: boolean
        /// example: false
        /// </summary>
        public bool? IsSecureConnection { get; set; }

        /// <summary>
        /// Content type of request
        /// type: keyword
        /// example: application/json, application/oct-stream
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Headers list of request in key=value format
        /// type: object
        /// </summary>
        public List<string> Headers { get; set; }

        /// <summary>
        /// Server variables list of request in key=value format
        /// type: object
        /// </summary>
        public List<string> ServerVariables { get; set; }

        /// <summary>
        /// Cookies list of request in key=value format
        /// type: object
        /// </summary>
        public List<string> Cookies { get; set; }

        /// <summary>
        /// Keys list of files uploaded by request
        /// type: object
        /// </summary>
        public List<string> Files { get; set; }
       
        /// <summary>
        /// Form posted data of request in key=value format
        /// type: object
        /// </summary>
        public List<string> Form { get; set; }
  
        /// <summary>
        /// Content length of data posted by request (in bytes)
        /// type: long
        /// example: 1024
        /// </summary>
        public long? ContentLength { get; set; }
        
        public HttpBodyModel Body { get; set; }       

        /// <summary>
        /// Total size in bytes of the request (body and headers).
        /// type: long
        /// example: 1437
        /// </summary>
        public long? Bytes { get; set; }       

        /// <summary>
        /// Referrer for this HTTP request.
        /// type: keyword
        /// example: https://blog.example.com/
        /// </summary>
        public string Referrer { get; set; }
    }
}