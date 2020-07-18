using System;
using Microsoft.AspNetCore.Http;
using Serilog.Configuration;
using Serilog.Enrichers;

namespace Serilog
{
    public static class LoggerEcsEnricherConfigurationExtensions
    {
        public static LoggerConfiguration WithEcs(this LoggerEnrichmentConfiguration enrich, 
            IHttpContextAccessor contextAccessor)
        {
            if (enrich == null)
                throw new ArgumentNullException(nameof(enrich));

            return enrich.With(new WithEcsEnricher(contextAccessor.HttpContext));
        }
    }
}