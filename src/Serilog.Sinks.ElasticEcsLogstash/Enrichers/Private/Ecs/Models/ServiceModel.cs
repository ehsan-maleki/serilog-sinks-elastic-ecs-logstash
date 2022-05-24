namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The service fields describe the service for or from which the data was collected.
    /// These fields help you find and correlate logs for a specific service and version.
    /// </summary>
    public class ServiceModel
    {
        /// <summary>
        /// Ephemeral identifier of this service (if one exists).
        /// This id normally changes across restarts, but service.id does not.
        /// type: keyword
        /// example: 8a4f500f
        /// </summary>
        public string EphemeralId { get; set; }       
       
        /// <summary>
        /// Unique identifier of the running service.
        /// This id should uniquely identify this service. This makes it possible to correlate logs and metrics for one specific service.
        /// Example: If you are experiencing issues with one redis instance, you can filter on that id to see metrics and logs for that single instance.
        /// type: keyword
        /// example: d37e5ebfe0ae6c4972dbe9f0174a1637bb8247f6
        /// </summary>
        public string Id { get; set; }       
       
        /// <summary>
        /// Name of the service data is collected from.
        /// The name of the service is normally user given.
        /// This allows if two instances of the same service are running on the same machine they can be differentiated by the service.name.
        /// Also it allows for distributed services that run on multiple hosts to correlate the related instances based on the name.
        /// In the case of Elasticsearch the service.name could contain the cluster name.
        /// For Beats the service.name is by default a copy of the service.type field if no name is specified.
        /// type: keyword
        /// example: elasticsearch-metrics
        /// </summary>
        public string Name { get; set; }       
       
        /// <summary>
        /// Current state of the service.
        /// type: keyword
        /// </summary>
        public string State { get; set; }       
       
        /// <summary>
        /// The type of the service data is collected from.
        /// The type can be used to group and correlate logs and metrics from one service type.
        /// Example: If logs or metrics are collected from Elasticsearch, service.type would be elasticsearch.
        /// type: keyword
        /// example: elasticsearch
        /// </summary>
        public string Type { get; set; }       
       
        /// <summary>
        /// Version of the service the data was collected from.
        /// This allows to look at a data set only for a specific version of a service.
        /// type: keyword
        /// example: 3.2.4
        /// </summary>
        public string Version { get; set; }       
    }
}