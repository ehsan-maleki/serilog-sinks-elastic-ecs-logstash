namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class ThreadModel
    {
        /// <summary>
        /// Thread ID.
        /// type: long
        /// example: 4242
        /// </summary>
        public long? Id { get; set; }       
    }
}