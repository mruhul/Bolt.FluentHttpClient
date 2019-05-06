using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpClientPerfLogHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestLog.Info($"HttpRequest starting | Url: {request.RequestUri} | Method: {request.Method}");

            var sw = Stopwatch.StartNew();

            var response = await base.SendAsync(request, cancellationToken);

            sw.Stop();

            HttpRequestLog.Info($"HttpRequest completed | StatusCode: {response.StatusCode} | Url : {request.RequestUri} | Method: {request.Method} | TimeTaken: {sw.ElapsedMilliseconds}ms");

            return response;
        }
    }
}
