namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The agent fields contain the data about the software entity, if any, that collects, detects,
    /// or observes events on a host, or takes measurements on a host.
    /// Examples include Beats. Agents may also run on observers.
    /// ECS agent.* fields shall be populated with details of the agent running on
    /// the host or observer where the event happened or the measurement was taken.
    /// </summary>
    public class AgentModel
    {
        /// <summary>
        /// This id normally changes across restarts, but agent.id does not.
        /// type: keyword
        /// example: 8a4f500f
        /// </summary>
        public string EphemeralId { get; set; }

        /// <summary>
        /// Unique identifier of this agent (if one exists).
        /// Example: For Beats this would be beat.id.
        /// type: keyword
        /// example: 8a4f500d
        /// </summary>
        public string Id { get; set; }

        
        /// <summary>
        /// Custom name of the agent.
        /// This is a name that can be given to an agent. This can be helpful if for example two Filebeat instances
        /// are running on the same host but a human readable separation is needed on which Filebeat instance data is coming from.
        /// If no name is given, the name is often left empty.
        /// type: keyword
        /// example: foo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the agent.
        /// The agent type stays always the same and should be given by the agent used. In case of Filebeat
        /// the agent would always be Filebeat also if two Filebeat instances are run on the same machine.
        /// type: keyword
        /// example: filebeat
        /// </summary>
        public string Type { get; set; }
       
        /// <summary>
        /// Root path of application.
        /// type: keyword
        /// </summary>
        public string ApplicationPath { get; set; }       

        /// <summary>
        /// Version of the agent.
        /// type: keyword
        /// example: 6.0.0-rc2
        /// </summary>
        public string Version { get; set; }
    }
}