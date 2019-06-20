namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Fields related to the cloud or infrastructure the events are coming from.
    /// </summary>
    public class CloudModel
    {
        /// <summary>
        /// The cloud account or organization id used to identify different entities in a multi-tenant environment.
        /// </summary>
        public AccountModel Account { get; set; }

        /// <summary>
        /// Availability zone in which this host is running.
        /// type: keyword
        /// example: us-east-1c
        /// </summary>
        public string AvailabilityZone { get; set; }
      
        /// <summary>
        /// Name of the cloud provider. Example values are aws, azure, gcp, or digitalocean.
        /// type: keyword
        /// example: aws
        /// </summary>
        public string Provider { get; set; }
      
        /// <summary>
        /// Region in which this host is running.
        /// type: keyword
        /// example: us-east-1
        /// </summary>
        public string Region { get; set; }
    }
}