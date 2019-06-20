namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class MachineModel
    {
        /// <summary>
        /// Machine type of the host machine.
        /// type: keyword
        /// example: t2.medium
        /// </summary>
        public string Type { get; set; }
    }
}