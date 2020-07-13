namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class HttpBodyModel
    {
        /// <summary>
        /// Size in bytes of the request body.
        /// type: long
        /// example: 887
        /// </summary>
        public long? Bytes { get; set; }       
       
        /// <summary>
        /// The full HTTP request body.
        /// type: keyword
        /// example: Hello world
        /// </summary>
        public string Content { get; set; }       
    }
}