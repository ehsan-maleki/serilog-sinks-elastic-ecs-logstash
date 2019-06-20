namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// These fields can represent errors of any kind.
    /// Use them for errors that happen while fetching events or in cases where the event itself contains an error.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error code describing the error.
        /// type: keyword
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Unique identifier for the error.
        /// type: keyword
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Error message.
        /// type: text
        /// </summary>
        public string Message { get; set; }
      
        /// <summary>
        /// Error message.
        /// type: text
        /// </summary>
        public string StackTrace { get; set; }
    }
}