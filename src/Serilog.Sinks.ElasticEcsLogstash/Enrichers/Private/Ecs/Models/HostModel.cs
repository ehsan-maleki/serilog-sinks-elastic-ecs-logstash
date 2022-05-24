namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// A host is defined as a general computing instance.
    /// ECS host.* fields should be populated with details about the host on which the event happened,
    /// or from which the measurement was taken. Host types include hardware, virtual machines,
    /// Docker containers, and Kubernetes nodes.
    /// </summary>
    public class HostModel
    {
        /// <summary>
        /// Operating system architecture.
        /// type: keyword
        /// example: x86_64
        /// </summary>
        public string Architecture { get; set; }       
       
        /// <summary>
        /// Hostname of the host.
        /// It normally contains what the hostname command returns on the host machine.
        /// type: keyword
        /// </summary>
        public string HostName { get; set; }       
       
        /// <summary>
        /// Unique host id.
        /// As hostname is not always unique, use values that are meaningful in your environment.
        /// Example: The current usage of beat.name.
        /// type: keyword
        /// </summary>
        public string Id { get; set; }       
       
        /// <summary>
        /// Host ip address.
        /// type: ip
        /// </summary>
        public string Ip { get; set; }       
       
        /// <summary>
        /// Host mac address.
        /// type: keyword
        /// </summary>
        public string Mac { get; set; }       
       
        /// <summary>
        /// Name of the host.
        /// It can contain what hostname returns on Unix systems, the fully qualified domain name,
        /// or a name specified by the user. The sender decides which value to use.
        /// type: keyword
        /// </summary>
        public string Name { get; set; }       
       
        /// <summary>
        /// Type of host.
        /// For Cloud providers this can be the machine type like t2.medium.
        /// If vm, this could be the container, for example, or other information meaningful in your environment.
        /// type: keyword
        /// </summary>
        public string Type { get; set; }       
       
        /// <summary>
        /// Fields describing a location.
        /// </summary>
        public GeoModel Geo { get; set; }       
       
        /// <summary>
        /// OS fields contain information about the operating system.
        /// </summary>
        public OperatingSystemModel Os { get; set; }       
       
        /// <summary>
        /// Fields to describe the user relevant to the event.
        /// </summary>
        public string User { get; set; } 
    }
}