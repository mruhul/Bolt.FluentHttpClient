using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.FluentHttpClient.CircuitBreaker
{
    public static class PropertyNames
    {
        public const string ServiceName = "_cb:servicename";
        public const string AppName = "_cb:appname";
        public const string CircuitKey = "_cb:circuitkey";
        public const string RequestId = "_cb:requestid";
    }
}
