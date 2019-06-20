namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The network is defined as the communication path over which a host or network event happens.
    /// The network.* fields should be populated with details about the network activity associated with an event.
    /// </summary>
    public class NetworkModel
    {
        /// <summary>
        /// A name given to an application level protocol. This can be arbitrarily assigned for things like microservices,
        /// but also apply to things like skype, icq, facebook, twitter. This would be used in situations where the vendor or
        /// service can be decoded such as from the source/dest IP owners, ports, or wire format.
        /// The field value must be normalized to lowercase for querying. See the documentation section "Implementing ECS".
        /// type: keyword
        /// example: aim
        /// </summary>
        public string Application { get; set; }       
       
        /// <summary>
        /// Total bytes transferred in both directions.
        /// If source.bytes and destination.bytes are known, network.bytes is their sum.
        /// type: long
        /// example: 368
        /// </summary>
        public long? Bytes { get; set; }       
       
        /// <summary>
        /// A hash of source and destination IPs and ports, as well as the protocol used in a communication.
        /// This is a tool-agnostic standard to identify flows.
        /// Learn more at https://github.com/corelight/community-id-spec.
        /// type: keyword
        /// example: 1:hO+sN4H+MG5MY/8hIrXPqc4ZQz0=
        /// </summary>
        public string CommunityId { get; set; }       
       
        /// <summary>
        /// Direction of the network traffic.
        /// Recommended values are:
        /// * inbound
        /// * outbound
        /// * internal
        /// * external
        /// * unknown
        /// When mapping events from a host-based monitoring context, populate this field from the host’s point of view.
        /// When mapping events from a network or perimeter-based monitoring context,
        /// populate this field from the point of view of your network perimeter.
        /// type: keyword
        /// example: inbound
        /// </summary>
        public string Direction { get; set; }       
       
        /// <summary>
        /// Host IP address when the source IP address is the proxy.
        /// type: ip
        /// example: 192.1.1.2
        /// </summary>
        public string ForwardedIp { get; set; }       
       
        /// <summary>
        /// IANA Protocol Number (https://www.iana.org/assignments/protocol-numbers/protocol-numbers.xhtml).
        /// Standardized list of protocols.
        /// This aligns well with NetFlow and sFlow related logs which use the IANA Protocol Number.
        /// type: keyword
        /// example: 6
        /// </summary>
        public string IanaNumber { get; set; }       
       
        /// <summary>
        /// Name given by operators to sections of their network.
        /// type: keyword
        /// example: Guest Wifi
        /// </summary>
        public string Name { get; set; }       
       
        /// <summary>
        /// Total packets transferred in both directions.
        /// If source.packets and destination.packets are known, network.packets is their sum.
        /// type: long
        /// example: 24
        /// </summary>
        public long? Packets { get; set; }       
       
        /// <summary>
        /// L7 Network protocol name. ex. http, lumberjack, transport protocol.
        /// The field value must be normalized to lowercase for querying. See the documentation section "Implementing ECS".
        /// type: keyword
        /// example: http
        /// </summary>
        public string Protocol { get; set; }       
       
        /// <summary>
        /// Same as network.iana_number, but instead using the Keyword name of the transport layer (udp, tcp, ipv6-icmp, etc.)
        /// The field value must be normalized to lowercase for querying. See the documentation section "Implementing ECS".
        /// type: keyword
        /// example: tcp
        /// </summary>
        public string Transport { get; set; }       
       
        /// <summary>
        /// In the OSI Model this would be the Network Layer. ipv4, ipv6, ipsec, pim, etc
        /// The field value must be normalized to lowercase for querying. See the documentation section "Implementing ECS".
        /// type: keyword
        /// example: ipv4
        /// </summary>
        public string Type { get; set; }
    }
}