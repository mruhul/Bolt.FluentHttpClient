using Bolt.CircuitBreaker.Abstracts;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.CircuitBreaker
{
    public class HttpClientCircuitBreakerHandler : DelegatingHandler
    {
        private readonly ICircuitBreaker _circuitBreaker;

        public HttpClientCircuitBreakerHandler(ICircuitBreaker circuitBreaker)
        {
            _circuitBreaker = circuitBreaker;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = GetPropertyValue<string>(request, PropertyNames.CircuitKey);
            var requestId = GetPropertyValue<string>(request, PropertyNames.RequestId);
            var appName = GetPropertyValue<string>(request, PropertyNames.AppName);
            var serviceName = GetPropertyValue<string>(request, PropertyNames.ServiceName);

            if (string.IsNullOrWhiteSpace(key))
            {
                key = request.RequestUri.Host;
            }

            var circuitRequest = new CircuitRequest
            {
                CircuitKey = key,
                RequestId = requestId,
                Retry = 0,
                Timeout = TimeSpan.Zero
            };

            circuitRequest.Context.SetAppName(appName);
            circuitRequest.Context.SetServiceName(serviceName);

            var cr = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(circuitRequest, async (cx) =>
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if(response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                || response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout
                || response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                || response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    response.Dispose();

                    throw new Exception($"Response failed with status code {response.StatusCode}");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
                {
                    response.Dispose();

                    throw new TimeoutException("Raising timeout to break circuit");
                }

                return response;
            });


            return cr.Value;

            //HttpRequestLog.Info($"HttpRequest starting | Url: {request.RequestUri} | Method: {request.Method}");

            //var sw = Stopwatch.StartNew();

            //var response = await base.SendAsync(request, cancellationToken);

            //sw.Stop();

            //HttpRequestLog.Info($"HttpRequest completed | StatusCode: {response.StatusCode} | Url : {request.RequestUri} | Method: {request.Method} | TimeTaken: {sw.ElapsedMilliseconds}ms");

            //return response;
        }

        private T GetPropertyValue<T>(HttpRequestMessage msg, string key)
        {
            return msg.Properties.TryGetValue(key, out var result) ? (T)result : default(T);
        }
    }
}
