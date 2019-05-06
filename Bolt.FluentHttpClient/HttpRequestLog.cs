using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Bolt.FluentHttpClient.Tests")]

namespace Bolt.FluentHttpClient
{
    public static class HttpRequestLog
    {
        private static ILogger _logger = null;

        public static void Init(ILogger logger)
        {
            _logger = logger;
        }

        internal static void Trace(string message)
        {
            _logger?.LogTrace(message);
        }

        internal static void Trace(string message, params object[] args)
        {
            _logger?.LogTrace(message, args);
        }

        internal static void Info(string message)
        {
            _logger?.LogInformation(message);
        }

        internal static void Info(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }

        internal static void Error(string message)
        {
            _logger?.LogError(message);
        }

        internal static void Error(string message, params object[] args)
        {
            _logger?.LogError(message, args);
        }

        internal static void Error(Exception e, string message)
        {
            _logger?.LogError(e, message);
        }

        internal static void Error(Exception e, string message, params object[] args)
        {
            _logger?.LogError(e, message, args);
        }
    }
}
