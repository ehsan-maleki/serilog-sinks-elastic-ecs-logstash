namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// A Server is defined as the responder in a network connection for events regarding sessions, connections, or bidirectional flow records.
    /// For TCP events, the server is the receiver of the initial SYN packet(s) of the TCP connection.
    /// For other protocols, the server is generally the responder in the network transaction.
    /// Some systems actually use the term "responder" to refer the server in TCP connections.
    /// The server fields describe details about the system acting as the server in the network event.
    /// Server fields are usually populated in conjunction with client fields. Server fields are generally not populated for packet-level events.
    /// Client / server representations can add semantic context to an exchange, which is helpful to visualize the data in certain situations.
    /// If your context falls in that category, you should still ensure that source and destination are filled appropriately.
    /// </summary>
    public class ServerModel
    {
        /// <summary>
        /// Some event server addresses are defined ambiguously.
        /// The event will sometimes list an IP, a domain or a unix socket. You should always store the raw address in the .address field.
        /// Then it should be duplicated to .ip or .domain, depending on which one it is.
        /// type: keyword
        /// </summary>
        public string Address { get; set; }       
       
        /// <summary>
        /// Bytes sent from the server to the client.
        /// type: long
        /// example: 184
        /// </summary>
        public long? Bytes { get; set; }       
       
        /// <summary>
        /// Server domain.
        /// type: keyword
        /// </summary>
        public string Domain { get; set; }       
       
        /// <summary>
        /// IP address of the server.
        /// Can be one or multiple IPv4 or IPv6 addresses.
        /// type: ip
        /// </summary>
        public string Ip { get; set; }       
       
        /// <summary>
        /// MAC address of the server.
        /// type: keyword
        /// </summary>
        public string Mac { get; set; }       
       
        /// <summary>
        /// ackets sent from the server to the client.
        /// type: long
        /// example: 12
        /// </summary>
        public long? Packets { get; set; }       
       
        /// <summary>
        /// Port of the server.
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
        public string User { get; set; } 
    }
}