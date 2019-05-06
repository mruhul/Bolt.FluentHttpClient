using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fake
{
    public class MockResponse
    {
        public string RequestUrl { get; set; }
        public UrlMatchType UrlMatchType { get; set; }
        public string RequestMethod { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string ContentType { get; set; }
        public object Content { get; set; }
    }

    public enum UrlMatchType
    {
        Exact,
        Exists,
        EndsWith,
        StartsWith,
        Regex
    }

    public class MockContent
    {
        public string ContentType { get; set; }
        public object Content { get; set; }
    }

    public class LocalFileBasedFakeResponse : IFakeResponse
    {
        private readonly Lazy<IEnumerable<MockResponse>> _data;
        private readonly IFakeResponseStore _responseStore;
        private readonly ILogger<LocalFileBasedFakeResponse> _logger;

        public LocalFileBasedFakeResponse(IFakeResponseStore responseStore, ILogger<LocalFileBasedFakeResponse> logger)
        {
            _data = new Lazy<IEnumerable<MockResponse>>(() => _responseStore.Get());
            _responseStore = responseStore;
            _logger = logger;
        }

        public Task<HttpResponseMessage> Get(HttpRequestMessage msg)
        {
            var mocks = _data.Value;

            if (mocks == null) return Task.FromResult<HttpResponseMessage>(null);

            var response = mocks.Where(x => string.Equals(x.RequestMethod, msg.Method.ToString(), StringComparison.OrdinalIgnoreCase) && IsUrlMatch(x.UrlMatchType, msg.RequestUri, x.RequestUrl)).FirstOrDefault();

            if (response == null)
            {
                _logger.LogWarning($"No Fake response match with the request for {msg.RequestUri.OriginalString}");

                return Task.FromResult<HttpResponseMessage>(null);
            }

            var result = new HttpResponseMessage
            {
                StatusCode = response.StatusCode
            };

            if(response.Headers != null)
            {
                foreach(var header in response.Headers)
                {
                    result.Headers.Add(header.Key, header.Value);
                }
            }

            if(response.Content != null)
            {
                var content = JsonConvert.SerializeObject(response.Content);

                result.Content = new StringContent(content, Encoding.UTF8, response.ContentType ?? "application/json");
            }

            return Task.FromResult(result);
        }

        private bool IsUrlMatch(UrlMatchType matchType, Uri requestUri, string mockUrl)
        {
            switch(matchType)
            {
                case UrlMatchType.Exact:
                    {
                        return string.Equals(requestUri.OriginalString, mockUrl, StringComparison.OrdinalIgnoreCase);
                    }
                case UrlMatchType.Exists:
                    {
                        return requestUri.OriginalString.IndexOf(mockUrl, StringComparison.OrdinalIgnoreCase) != -1;
                    }
                case UrlMatchType.StartsWith:
                    {
                        return requestUri.OriginalString.StartsWith(mockUrl, StringComparison.OrdinalIgnoreCase);
                    }
                case UrlMatchType.EndsWith:
                    {
                        return requestUri.OriginalString.EndsWith(mockUrl, StringComparison.OrdinalIgnoreCase);
                    }
                case UrlMatchType.Regex:
                    {
                        return Regex.IsMatch(mockUrl, requestUri.OriginalString, RegexOptions.IgnoreCase);
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
