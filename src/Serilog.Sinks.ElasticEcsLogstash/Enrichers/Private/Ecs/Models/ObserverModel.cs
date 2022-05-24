namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// An observer is defined as a special network, security, or application device used to detect, observe,
    /// or create network, security, or application-related events and metrics.
    /// This could be a custom hardware appliance or a server that has been configured to run special network, security, or application software.
    /// Examples include firewalls, intrusion detection/prevention systems, network monitoring sensors, web application firewalls,
    /// data loss prevention systems, and APM servers.
    /// The observer.* fields shall be populated with details of the system, if any, that detects, observes and/or creates a network,
    /// security, or application event or metric.
    /// Message queues and ETL components used in processing events or metrics are not considered observers in ECS.
    /// </summary>
    public class ObserverModel
    {
        /// <summary>
        /// Hostname of the observer.
        /// type: keyword
        /// </summary>
        public string HostName { get; set; }       
       
        /// <summary>
        /// IP address of the observer.
        /// type: ip
        /// </summary>
        public string Ip { get; set; }       
       
        /// <summary>
        /// MAC address of the observer
        /// type: keyword
        /// </summary>
        public string Mac { get; set; }       
       
        /// <summary>
        /// Observer serial number.
        /// type: keyword
        /// </summary>
        public string SerialNumber { get; set; }       
       
        /// <summary>
        /// The type of the observer the data is coming from.
        /// There is no predefined list of observer types. Some examples are forwarder, firewall, ids, ips, proxy, poller, sensor, APM server.
        /// type: keyword
        /// example: firewall
        /// </summary>
        public string Type { get; set; }       
       
        /// <summary>
        /// observer vendor information.
        /// type: keyword
        /// </summary>
        public string Vendor { get; set; }       
       
        /// <summary>
        /// Observer version.
        /// type: keyword
        /// </summary>
        public string Version { get; set; }       
        
        /// <summary>
        /// Fields describing a location.
        /// </summary>
        public GeoModel Geo { get; set; }       
       
        /// <summary>
        /// OS fields contain information about the operating system.
        /// </summary>
        public OperatingSystemModel Os { get; set; }       
    }
}