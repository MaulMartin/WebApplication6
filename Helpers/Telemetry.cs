using System.Diagnostics;

namespace WebApplication6.Helpers
{
    public static  class Telemetry
    {
        public static readonly ActivitySource MyActivitySource = new(Constants.ServiceName);
    }
}
