using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpClientPerfLogHandler : DelegatingHandler
    {
        private readonly ILogger<HttpClientPerfLogHandler> _logger;

        public HttpClientPerfLogHandler(ILogger<HttpClientPerfLogHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            var response = await base.SendAsync(request, cancellationToken);

            sw.Stop();

            _logger.LogInformation("{0}|{1}|{2}ms|{3}", request.RequestUri, request.Method, sw.ElapsedMilliseconds, response.StatusCode);

            return response;
        }
    }
}
