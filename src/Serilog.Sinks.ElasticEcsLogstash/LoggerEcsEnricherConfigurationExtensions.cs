using System;
using Serilog.Configuration;
using Serilog.Enrichers;

namespace Serilog
{
    public static class LoggerEcsEnricherConfigurationExtensions
    {
        public static LoggerConfiguration WithEcs(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With<WithEcsEnricher>();
        }
    }
}