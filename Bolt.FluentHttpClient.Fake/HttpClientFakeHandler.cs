using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fake
{
    public class HttpClientFakeHandler : DelegatingHandler
    {
        private readonly ILogger<HttpClientFakeHandler> _logger;
        private readonly IEnumerable<IFakeResponse> _fakeResponses;
        private readonly IEnumerable<IResponseReceivedListener> _listeners;

        public HttpClientFakeHandler(
            ILogger<HttpClientFakeHandler> logger, 
            IEnumerable<IFakeResponse> fakeResponses,
            IEnumerable<IResponseReceivedListener> listeners)
        {
            _logger = logger;
            _fakeResponses = fakeResponses;
            _listeners = listeners;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"HttpRequestFake starting | Url: {request.RequestUri} | Method: {request.Method}");

            foreach(var fakeResponse in _fakeResponses)
            {
                var result = await fakeResponse.Get(request);

                if (result != null)
                {
                    _logger.LogTrace("Fake response provided. So skip calling original service");

                    return result;
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (_listeners == null || !_listeners.Any()) return response;

            var stringContent = await response.Content?.ReadAsStringAsync();

            foreach(var listener in _listeners)
            {
                await listener.Notify(request, CopyMessage(response, stringContent));
            }

            return CopyMessage(response, stringContent);
        }

        private HttpResponseMessage CopyMessage(HttpResponseMessage response, string stringContent)
        {
            var rsp = new HttpResponseMessage
            {
                RequestMessage = response.RequestMessage,
                ReasonPhrase = response.ReasonPhrase,
                StatusCode = response.StatusCode,
                Version = response.Version,
                Content = new StringContent(stringContent, Encoding.UTF8, response.Content.Headers.ContentType?.MediaType)
            };

            foreach (var header in response.Headers)
            {
                rsp.Headers.Add(header.Key, header.Value);
            }

            return rsp;
        }
    }
}
