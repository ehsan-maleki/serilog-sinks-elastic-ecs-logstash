using System;
using System.Collections.Generic;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    public class BaseModel
    {
        public DateTimeOffset Timestamp { get; set; }

        public List<KeyValuePair<string, string>> Labels { get; set; }

        public List<string> Tags { get; set; }

        public List<string> Payload { get; set; }

        public AgentModel Agent { get; set; }

        public ClientModel Client { get; set; }

        public CloudModel Cloud { get; set; }

        public ContainerModel Container { get; set; }

        public DestinationModel Destination { get; set; }

        public EcsModel Ecs { get; set; }

        public ErrorModel Error { get; set; }

        public EventModel Event { get; set; }

        public FileModel File { get; set; }

        public GeoModel Geo { get; set; }

        public GroupModel Group { get; set; }

        public HostModel Host { get; set; }

        public HttpModel Http { get; set; }

        public LogModel Log { get; set; }

        public NetworkModel Network { get; set; }

        public ObserverModel Observer { get; set; }

        public OrganizationModel Organization { get; set; }

        public OperatingSystemModel OperatingSystem { get; set; }

        public ProcessModel Process { get; set; }

        public ServerModel Server { get; set; }

        public SourceModel Source { get; set; }

        public UrlModel Url { get; set; }

        public UserModel User { get; set; }

        public UserAgentModel UserAgent { get; set; }
    }
}