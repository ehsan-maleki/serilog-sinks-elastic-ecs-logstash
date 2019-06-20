namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// Geo fields can carry data about a specific location related to an event.
    /// This geolocation information can be derived from techniques such as Geo IP, or be user-supplied.
    /// </summary>
    public class GeoModel
    {
        /// <summary>
        /// City name.
        /// type: keyword
        /// example: Montreal
        /// </summary>
        public string CityName { get; set; }       

        /// <summary>
        /// Name of the continent.
        /// type: keyword
        /// example: North America
        /// </summary>
        public string ContinentName { get; set; }       

        /// <summary>
        /// Country ISO code.
        /// type: keyword
        /// example: CA
        /// </summary>
        public string CountryIsoCode { get; set; }       

        /// <summary>
        /// Country name.
        /// type: keyword
        /// example: Canada
        /// </summary>
        public string CountryName { get; set; }       

        /// <summary>
        /// Longitude and latitude.
        /// type: geo_point
        /// example: { "lon": -73.614830, "lat": 45.505918 }
        /// </summary>
        public GeoLocationModel Location { get; set; }       

        /// <summary>
        /// User-defined description of a location, at the level of granularity they care about.
        /// Could be the name of their data centers, the floor number, if this describes a local physical entity, city names.
        /// Not typically used in automated geolocation.
        /// type: keyword
        /// example: boston-dc
        /// </summary>
        public string Name { get; set; }       

        /// <summary>
        /// Region ISO code.
        /// type: keyword
        /// example: CA-QC
        /// </summary>
        public string RegionIsoCode { get; set; }

        /// <summary>
        /// Region name.
        /// type: keyword
        /// example: Quebec
        /// </summary>
        public string RegionName { get; set; }
    }
}