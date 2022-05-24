namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Source fields describe details about the source of a packet/event.
    /// Source fields are usually populated in conjunction with destination fields.
    /// </summary>
    public class SourceModel
    {
        /// <summary>
        /// Some event source addresses are defined ambiguously.
        /// The event will sometimes list an IP, a domain or a unix socket.
        /// You should always store the raw address in the .address field.
        /// Then it should be duplicated to .ip or .domain, depending on which one it is.
        /// type: keyword
        /// </summary>
        public string Address { get; set; }       

        /// <summary>
        /// Bytes sent from the source to the destination.
        /// type: long
        /// example: 184
        /// </summary>
        public long? Bytes { get; set; }       

        /// <summary>
        /// Source domain.
        /// type: keyword
        /// </summary>
        public string Domain { get; set; }       

        /// <summary>
        /// IP address of the source.
        /// Can be one or multiple IPv4 or IPv6 addresses.
        /// type: ip
        /// </summary>
        public string Ip { get; set; }       

        /// <summary>
        /// MAC address of the source.
        /// type: keyword
        /// </summary>
        public string Mac { get; set; }       

        /// <summary>
        /// Packets sent from the source to the destination.
        /// type: long
        /// example: 12
        /// </summary>
        public long? Packets { get; set; }       

        /// <summary>
        /// Port of the source.
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