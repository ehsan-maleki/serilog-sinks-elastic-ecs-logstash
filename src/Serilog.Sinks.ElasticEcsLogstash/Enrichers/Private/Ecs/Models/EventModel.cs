using System;

namespace Serilog.Enrichers.Private.Ecs.Models
{
    /// <summary>
    /// The event fields are used for context information about the log or metric event itself.
    /// A log is defined as an event containing details of something that happened.
    /// Log events must include the time at which the thing happened.
    /// Examples of log events include a process starting on a host, a network packet being sent from a source to a destination,
    /// or a network connection between a client and a server being initiated or closed.
    /// A metric is defined as an event containing one or more numerical or categorical measurements and the time at which
    /// the measurement was taken. Examples of metric events include memory pressure measured on a host,
    /// or vulnerabilities measured on a scanned host.
    /// </summary>
    public class EventModel
    {
        public string Level { get; set; }
        /// <summary>
        /// The action captured by the event.
        /// This describes the information in the event. It is more specific than event.category.
        /// Examples are group-add, process-started, file-created. The value is normally defined by the implementer.
        /// type: keyword
        /// example: user-password-change
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Event category.
        /// This contains high-level information about the contents of the event. It is more generic than event.action,
        /// in the sense that typically a category contains multiple actions.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: user-management
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// event.created contains the date/time when the event was first read by an agent, or by your pipeline.
        /// This field is distinct from @timestamp in that @timestamp typically contain the time extracted from the original event.
        /// In most situations, these two timestamps will be slightly different.
        /// The difference can be used to calculate the delay between your source generating an event,
        /// and the time when your agent first processed it.
        /// This can be used to monitor your agent’s or pipeline’s ability to keep up with your event source.
        /// In case the two timestamps are identical, @timestamp should be used.
        /// type: date
        /// </summary>
        public DateTime? Created { get; set; }

        /// <summary>
        /// Name of the dataset.
        /// The concept of a dataset (fileset / metricset) is used in Beats as a subset of modules.
        /// It contains the information which is currently stored in metricset.name and metricset.module or fileset.name.
        /// type: keyword
        /// example: stats
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// Duration of the event in nanoseconds.
        /// If event.start and event.end are known this value should be the difference between the end and start time.
        /// type: long
        /// </summary>
        public long? Duration { get; set; }

        /// <summary>
        /// event.end contains the date when the event ended or when the activity was last observed.
        /// type: date
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Hash (perhaps logstash fingerprint) of raw field to be able to demonstrate log integrity.
        /// type: keyword
        /// example: 123456789012345678901234567890ABCD
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Unique ID to describe the event.
        /// type: keyword
        /// example: 8a4f500d
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The kind of the event.
        /// This gives information about what type of information the event contains, without being specific to the contents of the event.
        /// Examples are event, state, alarm.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: state
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Name of the module this data is coming from.
        /// This information is coming from the modules used in Beats or Logstash.
        /// type: keyword
        /// example: mysql
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Raw text message of entire event. Used to demonstrate log integrity.
        /// This field is not indexed and doc_values are disabled. It cannot be searched, but it can be retrieved from _source.
        /// type: keyword
        /// example: Sep 19 08:26:10 host CEF:0&#124;Security&#124; threatmanager&#124;1.0&#124;100&#124; worm successfully stopped&#124;10&#124;src=10.0.0.1 dst=2.1.2.2spt=1232
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// The outcome of the event.
        /// If the event describes an action, this fields contains the outcome of that action.
        /// Examples outcomes are success and failure.
        /// Warning: In future versions of ECS, we plan to provide a list of acceptable values for this field, please use with caution.
        /// type: keyword
        /// example: success
        /// </summary>
        public string Outcome { get; set; }

        /// <summary>
        /// Risk score or priority of the event (e.g. security solutions). Use your system’s original value here.
        /// type: float
        /// </summary>
        public float? RiskScore { get; set; }

        /// <summary>
        /// Normalized risk score or priority of the event, on a scale of 0 to 100.
        /// This is mainly useful if you use more than one system that assigns risk scores, and you want to see a normalized value across all systems.
        /// type: float
        /// </summary>
        public float? RiskScoreNorm { get; set; }

        /// <summary>
        /// Severity describes the original severity of the event.
        /// What the different severity values mean can very different between use cases.
        /// It’s up to the implementer to make sure severities are consistent across events.
        /// type: long
        /// example: 7
        /// </summary>
        public long? Severity { get; set; }

        /// <summary>
        /// event.start contains the date when the event started or when the activity was first observed.
        /// type: date
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// This field should be populated when the event’s timestamp does not include timezone information already (e.g. default Syslog timestamps).
        /// It’s optional otherwise.
        /// Acceptable timezone formats are: a canonical ID (e.g. "Europe/Amsterdam"), abbreviated (e.g. "EST") or an HH:mm differential (e.g. "-05:00").
        /// type: keyword
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Reserved for future usage.
        /// Please avoid using this field for user data.
        /// type: keyword
        /// </summary>
        public string Type { get; set; }
    }
}