using System.Linq;

namespace Serilog.Sinks.ElasticEcsLogstash.TextFormatters
{
    public static class KeyFormatter
    {
        public static string ToUnderscoreCase(this string str) {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}