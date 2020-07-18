using System.Linq;

namespace Serilog.Sinks.ElasticEcsLogstash.TextFormatters
{
    public static class KeyFormatter
    {
        public static string ToUnderscoreCase(this string str)
        {
            var result = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()));
            result = string.Join(".", result.ToLower().Split('.').Select(x => x.Trim(' ', '\t', '\n', '_')));
            return result;
        }
    }
}