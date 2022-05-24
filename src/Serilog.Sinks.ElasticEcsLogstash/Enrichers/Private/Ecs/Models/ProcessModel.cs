using System;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// These fields contain information about a process.
    /// These fields can help you correlate metrics information with a process id/name from a log message.
    /// The process.pid often stays in the metric itself and is copied to the global field for correlation.
    /// </summary>
    public class ProcessModel
    {
        /// <summary>
        /// Array of process arguments.
        /// May be filtered to protect sensitive information.
        /// type: keyword
        /// example: ['ssh', '-l', 'user', '10.0.0.16']
        /// </summary>
        public string[] Args { get; set; }       
       
        /// <summary>
        /// Absolute path to the process executable.
        /// type: keyword
        /// example: /usr/bin/ssh
        /// </summary>
        public string Executable { get; set; }       
       
        /// <summary>
        /// Process name.
        /// Sometimes called program name or similar.
        /// type: keyword
        /// example: ssh
        /// </summary>
        public string Name { get; set; }       
       
        /// <summary>
        /// Process id.
        /// type: long
        /// </summary>
        public long? Pid { get; set; }       
       
        /// <summary>
        /// Process parent id.
        /// type: long
        /// </summary>
        public long? Ppid { get; set; }       
       
        /// <summary>
        /// The time the process started.
        /// type: date
        /// example: 2016-05-23T08:05:34.853Z
        /// </summary>
        public DateTime? Start { get; set; }       
       
        public ThreadModel Thread { get; set; }       
       
        /// <summary>
        /// Process title.
        /// The proctitle, some times the same as process name.
        /// Can also be different: for example a browser setting its title to the web page currently opened.
        /// type: keyword
        /// </summary>
        public string Title { get; set; }       
       
        /// <summary>
        /// The working directory of the process.
        /// type: keyword
        /// example: /home/alice
        /// </summary>
        public string WorkingDirectory { get; set; }       
    }
}