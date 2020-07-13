namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Destination fields describe details about the destination of a packet/event.
    /// Destination fields are usually populated in conjunction with source fields.
    /// </summary>
    public class DestinationModel
    {
        /// <summary>
        /// Some event destination addresses are defined ambiguously.
        /// The event will sometimes list an IP, a domain or a unix socket.
        /// You should always store the raw address in the .address field.
        /// Then it should be duplicated to .ip or .domain, depending on which one it is.
        /// type: keyword
        /// </summary>
        public string Address { get; set; }       

        /// <summary>
        /// Bytes sent from the destination to the source.
        /// type: long
        /// example: 184
        /// </summary>
        public long? Bytes { get; set; }       

        /// <summary>
        /// Destination domain.
        /// type: keyword
        /// </summary>
        public string Domain { get; set; }       

        /// <summary>
        /// IP address of the destination.
        /// Can be one or multiple IPv4 or IPv6 addresses.
        /// type: ip
        /// </summary>
        public string Ip { get; set; }       

        /// <summary>
        /// MAC address of the destination.
        /// type: keyword
        /// </summary>
        public string Mac { get; set; }       

        /// <summary>
        /// Packets sent from the destination to the source.
        /// type: long
        /// example: 12
        /// </summary>
        public long? Packets { get; set; }       

        /// <summary>
        /// Port of the destination.
        /// type: long
        /// </summary>
        public long? Port { get; set; }       

        /// <summary>
        /// Fields describing a location.
        /// </summary>
        public GeoModel Geo { get; set; }

        /// <summary>
        /// Fields to describe the user relevant to the event.
        /// </summary>
        public UserModel User { get; set; }
    }
}