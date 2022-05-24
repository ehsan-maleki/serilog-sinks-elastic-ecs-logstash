namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class LogModel
    {
        /// <summary>
        /// Original log level of the log event.
        /// Some examples are warn, error, i.
        /// type: keyword
        /// example: err
        /// </summary>
        public string Level { get; set; }       
       
        /// <summary>
        /// This is the original log message and contains the full log message before splitting it up in multiple parts.
        /// In contrast to the message field which can contain an extracted part of the log message,
        /// this field contains the original, full log message.
        /// It can have already some modifications applied like encoding or new lines removed to clean up the log message.
        /// This field is not indexed and doc_values are disabled so it can’t be queried but the value can be retrieved from _source.
        /// type: keyword
        /// example: Sep 19 08:26:10 localhost My log
        /// </summary>
        public string Original { get; set; }
    }
}