namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// These fields can represent errors of any kind.
    /// Use them for errors that happen while fetching events or in cases where the event itself contains an error.
    /// </summary>
    public class EcsModel
    {
        /// <summary>
        /// ECS version this event conforms to. ecs.version is a required field and must exist in all events.
        /// When querying across multiple indices — which may conform to slightly different ECS versions —
        /// this field lets integrations adjust to the schema version of the events.
        /// type: keyword
        /// example: 1.0.0
        /// </summary>
        public string Version { get; } = "1.0.0";
    }
}