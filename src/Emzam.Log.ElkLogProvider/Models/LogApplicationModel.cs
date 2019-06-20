using System.Configuration;
using Emzam.Log.ElkLogProvider.Enum;

namespace Emzam.Log.ElkLogProvider.Models
{
    public class LogApplicationModel
    {
        /// <summary>
        /// Unique id of current application.
        /// Such as: JAAPI, JAFRONT, JACP and etc.
        /// Such as: Tg6G, N8Yh7, Bg7b5y and etc.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name of current applications.
        /// Such as: Jabama Api v1, Jabama Api v1.0.0 and etc.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Current applications type.
        /// Such as: Api, Windows Service and etc.
        /// </summary>
        public ApplicationTypes Type { get; set; }
        /// <summary>
        /// Current version of application in x.xy.xy format.
        /// Such as: 1.0.0, 1.15.8 and etc. 
        /// </summary>
        public string Version { get; set; }

        public LogApplicationModel()
        {
            Id = ConfigurationManager.AppSettings["ApplicationId"] ?? "Unknown";
            Name = ConfigurationManager.AppSettings["ApplicationName"] ?? "Unknown";

            var type = ConfigurationManager.AppSettings["ApplicationType"];
            Type = (ApplicationTypes) System.Enum.Parse(typeof(ApplicationTypes), type ?? "Unknown",true);
            
            Version = ConfigurationManager.AppSettings["ApplicationVersion"] ?? "1.0.0";
        }

        public LogApplicationModel(LogApplicationModel data)
            : this()
        {
            Id = string.IsNullOrEmpty(data.Id) ? Id : data.Id;
            Name = string.IsNullOrEmpty(data.Name) ? Name : data.Name;
            Type = data.Type == ApplicationTypes.Unknown ? Type : data.Type;
            Version = string.IsNullOrEmpty(data.Version) ? Version : data.Version;
        }
    }
}