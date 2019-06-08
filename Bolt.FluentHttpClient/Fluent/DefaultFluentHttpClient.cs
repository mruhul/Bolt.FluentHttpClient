using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bolt.FluentHttpClient.Abstracts;

namespace Bolt.FluentHttpClient.Fluent
{
    internal class FluentHttpClientImp : ICollectUrl, IHaveUrl, IHaveProperties, IHaveOnFailure, IHaveHeader, IHaveTimeout, IHaveRetryCount, IRequestExecute
    {
        private readonly IFluentHttpClient _http;
        private string _url;
        private Dictionary<string, object> _properties;
        private readonly List<HttpHeader> _headers = new List<HttpHeader>();
        private Func<HttpResponseContent, IHttpSerializer, Task> _onFailure = null;
        private Dictionary<HttpStatusCode, Func<HttpResponseContent, IHttpSerializer, Task>> _errors;


        private int? _retryCount;
        private TimeSpan? _timeout;


        internal FluentHttpClientImp(IFluentHttpClient http)
        {
            _http = http;
        }

        public IHaveUrl Url(string url)
        {
            _url = url;
            return this;
        }

        public IHaveRetryCount Retry(int retry)
        {
            _retryCount = retry;
            return this;
        }

        public IHaveTimeout Timeout(TimeSpan timeSpan)
        {
            _timeout = timeSpan;
            return this;
        }
        public IHaveTimeout TimeoutInMilliseconds(int milliseconds)
        {
            _timeout = TimeSpan.FromMilliseconds(milliseconds);
            return this;
        }


        public IHaveHeader Header(HttpHeader header)
        {
            if (header?.Value == null) return this;

            _headers.Add(header);

            return this;
        }

        public IHaveHeader Header(string name, string value)
        {
            return Header(new HttpHeader { Name = name, Value = value });
        }

        public IHaveHeader Headers(IEnumerable<HttpHeader> headers)
        {
            if (headers == null) return this;

            foreach(var header in headers)
            {
                if (header.Value == null) continue;

                _headers.Add(header);
            }

            return this;
        }


        public Task<HttpRequestOutput> DeleteAsync()
        {
            return _http.SendAsync(new HttpRequestInput
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Delete,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput<T>> GetAsync<T>()
        {
            return _http.SendAsync<T>(new HttpRequestInput
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Get,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput> PostAsync()
        {
            return _http.SendAsync(new HttpRequestInput
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput<TOutput>> PostAsync<TOutput>()
        {
            return _http.SendAsync<TOutput>(new HttpRequestInput
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput<TOutput>> PostAsync<TInput, TOutput>(TInput input)
        {
            return _http.SendAsync<TInput,TOutput>(new HttpRequestInput<TInput>
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Content = input,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput> PostAsync<TInput>(TInput input)
        {
            return _http.SendAsync(new HttpRequestInput<TInput>
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Content = input,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput> PutAsync()
        {
            return _http.SendAsync(new HttpRequestInput
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput<TOutput>> PutAsync<TOutput>()
        {
            return _http.SendAsync<TOutput>(new HttpRequestInput
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Properties = _properties
            });
        }


        public Task<HttpRequestOutput> PutAsync<TInput>(TInput input)
        {
            return _http.SendAsync(new HttpRequestInput<TInput>
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Content = input,
                Properties = _properties
            });
        }

        public Task<HttpRequestOutput<TOutput>> PutAsync<TInput, TOutput>(TInput input)
        {
            return _http.SendAsync<TInput, TOutput>(new HttpRequestInput<TInput>
            {
                Uri = new Uri(_url),
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                Retry = _retryCount,
                Timeout = _timeout,
                Content = input,
                Properties = _properties
            });
        }

        private Func<HttpResponseContent, IHttpSerializer, Task> BuildOnFailure()
        {
            if (_onFailure != null) return _onFailure;
            if (_errors == null) return null;

            return (rsp, sr) => {
                if(_errors.TryGetValue(rsp.StatusCode, out var errFunc))
                {
                    return errFunc(rsp, sr);
                }

                return Task.CompletedTask;
            };
        }

        public IHaveOnFailure OnFailure(Func<HttpResponseContent, IHttpSerializer, Task> onFailureAsync)
        {
            _onFailure = onFailureAsync;
            return this;
        }

        public IHaveOnFailure OnFailure(HttpStatusCode statusCode, Func<HttpResponseContent, IHttpSerializer, Task> onFailureAsync)
        {
            if (_errors == null) _errors = new Dictionary<HttpStatusCode, Func<HttpResponseContent, IHttpSerializer, Task>>();

            _errors[statusCode] = (rsp, serializer) => {
                return onFailureAsync(rsp, serializer);
            };

            return this;
        }

        public IHaveOnFailure OnFailure<TError>(HttpStatusCode statusCode, Action<TError> onBadRequest)
        {
            if (_errors == null) _errors = new Dictionary<HttpStatusCode, Func<HttpResponseContent, IHttpSerializer, Task>>();

            _errors[statusCode] = (rsp, serializer) => {
                onBadRequest(serializer.Deserialize<TError>(rsp.ContentStream));
                return Task.CompletedTask;
            };

            return this;
        }

        public IHaveOnFailure OnBadRequest<TError>(Action<TError> onBadRequest)
        {
            return OnFailure(HttpStatusCode.BadRequest, onBadRequest);
        }

        public IHaveProperties Properties(Dictionary<string, object> properties)
        {
            if (properties == null) return this;

            if (_properties == null) _properties = new Dictionary<string, object>();

            foreach(var item in properties)
            {
                _properties[item.Key] = item.Value;
            }

            return this;
        }

        public IHaveProperties Property(string name, object value)
        {
            if (_properties == null) _properties = new Dictionary<string, object>();

            _properties[name] = value;

            return this;
        }
    }
}
