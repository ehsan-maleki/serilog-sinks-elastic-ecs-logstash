namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The group fields are meant to represent groups that are relevant to the event.
    /// </summary>
    public class GroupModel
    {
        /// <summary>
        /// Unique identifier for the group on the system/platform.
        /// type: keyword
        /// </summary>
        public string Id { get; set; }       
       
        /// <summary>
        /// Name of the group.
        /// type: keyword
        /// </summary>
        public string Name { get; set; }       
    }
}