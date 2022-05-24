namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class DeviceModel
    {
        /// <summary>
        /// Name of the device.
        /// type: keyword
        /// example: iPhone
        /// </summary>
        public string Name { get; set; }       

        /// <summary>
        /// Manufacturer of the device.
        /// type: keyword
        /// example: Apple
        /// </summary>
        public string Manufacturer { get; set; }       
    }
}