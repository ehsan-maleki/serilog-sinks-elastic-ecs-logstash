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
        /// Such as: iCompany Api v1, iCompany Api v1.0.0 and etc.
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
        
        /// <summary>
        /// Dns name of server hosting application
        /// </summary>
        public string Server { get; set; }

        public LogApplicationModel()
        {
            Id = "Unknown";
            Name = "Unknown";
            Type = ApplicationTypes.Unknown;
            Version = "0.0.0";
        }

        public LogApplicationModel(LogApplicationModel data)
            : this()
        {
            Id = string.IsNullOrEmpty(data.Id) ? Id : data.Id;
            Name = string.IsNullOrEmpty(data.Name) ? Name : data.Name;
            Type = data.Type == ApplicationTypes.Unknown ? Type : data.Type;
            Version = string.IsNullOrEmpty(data.Version) ? Version : data.Version;
            Server = string.IsNullOrEmpty(data.Server) ? Server : data.Server;
        }
    }
}