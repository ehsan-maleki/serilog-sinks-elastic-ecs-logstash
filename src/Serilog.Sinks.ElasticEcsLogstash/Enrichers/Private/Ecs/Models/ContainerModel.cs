using System.Collections.Generic;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Container fields are used for meta information about the specific container that is the source of information.
    /// These fields help correlate data based containers from any runtime.
    /// </summary>
    public class ContainerModel
    {
        /// <summary>
        /// Unique container id.
        /// type: keyword
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the image the container was built on.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Image labels.
        /// </summary>
        public List<KeyValuePair<string, string>> Type { get; set; }

        /// <summary>
        /// Container name.
        /// type: keyword
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Runtime managing this container.
        /// type: keyword
        /// example: docker
        /// </summary>
        public string Runtime { get; set; }       
    }
}