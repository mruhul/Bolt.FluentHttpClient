using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpClientTimedoutHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timeout = request.GetPropertyValueOrDefault<TimeSpan>(Constants.PropertyNameTimeout);

            if (timeout == TimeSpan.Zero) return await base.SendAsync(request, cancellationToken);

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                cts.CancelAfter(timeout);

                try
                {
                    return await base.SendAsync(request, cts.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    return BuildTimedoutResponse(request, timeout);
                }
                catch (HttpRequestException e)
                    when (!cancellationToken.IsCancellationRequested
                        && e.InnerException != null
                        && e.InnerException is OperationCanceledException)
                {
                    return BuildTimedoutResponse(request, timeout);
                }
            }
        }


        private static HttpResponseMessage BuildTimedoutResponse(HttpRequestMessage request, TimeSpan timeout)
        {
            HttpRequestLog.Error($"Http request timedout after {timeout.TotalMilliseconds}ms | url: {request.RequestUri} | method:{request.Method}");

            var rsp = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.RequestTimeout,
                ReasonPhrase = "HttpClientTimedout"
            };

            rsp.Headers.Add(Constants.HeaderNameError, "HttpClient raised timeoutexception");

            return rsp;
        }
    }
}
