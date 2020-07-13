namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class InstanceModel
    {
        /// <summary>
        /// Instance ID of the host machine.
        /// type: keyword
        /// example: i-1234567890abcdef0
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Instance name of the host machine.
        /// type: keyword
        /// </summary>
        public string Name { get; set; }
    }
}