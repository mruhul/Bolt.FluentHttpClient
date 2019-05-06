using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpClientRetryHandler : DelegatingHandler
    {
        private static readonly HttpStatusCode[] StatusCodeToRetry = new HttpStatusCode[] {
            HttpStatusCode.BadGateway,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.ServiceUnavailable
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var retryCount = request.GetPropertyValueOrDefault<int>(Constants.PropertyNameRetryCount);

            HttpResponseMessage response = null;

            for (var i = 0; i < retryCount + 1; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (!ShouldRetry(response))
                {
                    return response;
                }
                else
                {
                    HttpRequestLog.Error($"Retrying {i+1} time(s) after http request failed with statuscode {response.StatusCode} | url: {request.RequestUri} | method:{request.Method}");
                }
            }

            return response;
        }

        protected bool ShouldRetry(HttpResponseMessage msg)
        {
            if (msg.IsSuccessStatusCode || msg.StatusCode == HttpStatusCode.BadRequest) return false;

            foreach (var code in StatusCodeToRetry)
            {
                if (code == msg.StatusCode)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
