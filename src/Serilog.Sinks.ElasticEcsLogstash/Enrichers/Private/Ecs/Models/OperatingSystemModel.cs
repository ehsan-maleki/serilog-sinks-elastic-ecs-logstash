namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The OS fields contain information about the operating system.
    /// </summary>
    public class OperatingSystemModel
    {
        /// <summary>
        /// OS family (such as redhat, debian, freebsd, windows).
        /// type: keyword
        /// example: debian
        /// </summary>
        public string Family { get; set; }       
       
        /// <summary>
        /// Operating system name, including the version or code name.
        /// type: keyword
        /// example: Mac OS Mojave
        /// </summary>
        public string Full { get; set; }       
       
        /// <summary>
        /// Operating system kernel version as a raw string.
        /// type: keyword
        /// example: 4.4.0-112-generic
        /// </summary>
        public string Kernel { get; set; }       
       
        /// <summary>
        /// Operating system name, without the version.
        /// type: keyword
        /// example: Mac OS X
        /// </summary>
        public string Name { get; set; }       
       
        /// <summary>
        /// Operating system platform (such centos, ubuntu, windows).
        /// type: keyword
        /// example: darwin
        /// </summary>
        public string Platform { get; set; }       
       
        /// <summary>
        /// Operating system version as a raw string.
        /// type: keyword
        /// example: 10.14.1
        /// </summary>
        public string Version { get; set; }       
    }
}