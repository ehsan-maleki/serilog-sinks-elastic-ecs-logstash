using System;
using Emzam.Log.ElkEcsLogBySerilogProvider.Enum;

namespace Emzam.Log.ElkEcsLogBySerilogProvider.Models
{
    public class JabamaActionModel
    {
        /// <summary>
        /// Unique ID of log
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Such as Audit, Information 
        /// </summary>
        public LogCategories Category { get; } = LogCategories.Error;
        /// <summary>
        /// Unique name of current action
        /// Such as "Login", "" 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// What this log contains?
        /// Such as "Login Audit"
        /// </summary>
        public string Kind { get; set; }
        /// <summary>
        /// Severity of action
        /// </summary>
        public Severities Severity { get; set; } = Severities.High;
        
        /// <summary>
        /// Extra data attached to this log.
        /// Data should be serialized.
        /// </summary>
        public string Payload { get; set; }
        
        /// <summary>
        /// Application exception attached to log.
        /// </summary>
        public Exception Exception { get; set; }
    }

}