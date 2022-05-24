namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Name of the image the container was built on.
    /// </summary>
    public class ContainerImageModel
    {
        /// <summary>
        /// Name of the image the container was built on.
        /// type: keyword
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Container image tag.
        /// type: keyword
        /// </summary>
        public string Tag { get; set; }
    }
}