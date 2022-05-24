using System.Collections.Generic;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class UserAgentModel
    {
        /// <summary>
        /// This user agent is on mobile device.
        /// type: boolean
        /// example: false
        /// </summary>
        public bool? IsMobileDevice { get; set; }
        
        public DeviceModel Device { get; set; }       

        /// <summary>
        /// Name of the user agent.
        /// type: keyword
        /// example: Safari
        /// </summary>
        public string Name { get; set; }       

        /// <summary>
        /// Unparsed version of the user_agent.
        /// type: keyword
        /// example: Mozilla/5.0 (iPhone; CPU iPhone OS 12_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// Platform name of user agent.
        /// type: keyword
        /// example: Postman App
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Screen width of user agent in pixels.
        /// type: long
        /// example: 1024
        /// </summary>
        public int? ScreenPixelsWidth { get; set; }

        /// <summary>
        /// Screen height of user agent in pixels.
        /// type: long
        /// example: 1024
        /// </summary>
        public int? ScreenPixelsHeight { get; set; }

        /// <summary>
        /// Type of the user agent.
        /// type: keyword
        /// </summary>
        public string Type { get; set; }
           
        /// <summary>
        /// Is user agent a crawler?
        /// type: boolean
        /// example: false
        /// </summary>
        public bool? IsCrawler { get; set; }
        
        /// <summary>
        /// Version of the user agent.
        /// type: keyword
        /// example: 12.0
        /// </summary>
        public string Version { get; set; }       
    }
}