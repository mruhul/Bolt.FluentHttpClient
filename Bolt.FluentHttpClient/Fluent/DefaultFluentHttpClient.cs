﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bolt.FluentHttpClient.Abstracts;

namespace Bolt.FluentHttpClient.Fluent
{
    internal class FluentHttpClientImp : ICollectUrl, IHaveUrl, IHaveOnFailure, IHaveHeader, IHaveTimeout, IHaveRetryCount, IRequestExecute
    {
        private readonly IFluentHttpClient _http;
        private string _url;
        private readonly List<HttpHeader> _headers = new List<HttpHeader>();
        private Func<HttpResponseContent, IHttpSerializer, Task> _onFailure = null;
        private Dictionary<HttpStatusCode, Func<HttpResponseContent, IHttpSerializer, Task>> _errors;


        private int _retryCount;
        private TimeSpan _timeout = TimeSpan.Zero;


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
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Delete,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout
            });
        }

        public Task<HttpRequestOutput<T>> GetAsync<T>()
        {
            return _http.SendAsync<T>(new HttpRequestInput
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Get,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout
            });
        }

        public Task<HttpRequestOutput> PostAsync()
        {
            return _http.SendAsync(new HttpRequestInput
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout
            });
        }

        public Task<HttpRequestOutput<TOutput>> PostAsync<TOutput>()
        {
            return _http.SendAsync<TOutput>(new HttpRequestInput
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout
            });
        }

        public Task<HttpRequestOutput<TOutput>> PostAsync<TInput, TOutput>(TInput input)
        {
            return _http.SendAsync<TInput,TOutput>(new HttpRequestInput<TInput>
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout,
                Content = input
            });
        }

        public Task<HttpRequestOutput> PostAsync<TInput>(TInput input)
        {
            return _http.SendAsync(new HttpRequestInput<TInput>
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Post,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout,
                Content = input
            });
        }

        public Task<HttpRequestOutput> PutAsync()
        {
            return _http.SendAsync(new HttpRequestInput
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout
            });
        }

        public Task<HttpRequestOutput<TOutput>> PutAsync<TOutput>()
        {
            return _http.SendAsync<TOutput>(new HttpRequestInput
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout
            });
        }


        public Task<HttpRequestOutput> PutAsync<TInput>(TInput input)
        {
            return _http.SendAsync(new HttpRequestInput<TInput>
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout,
                Content = input
            });
        }

        public Task<HttpRequestOutput<TOutput>> PutAsync<TInput, TOutput>(TInput input)
        {
            return _http.SendAsync<TInput, TOutput>(new HttpRequestInput<TInput>
            {
                Url = _url,
                Headers = _headers,
                Method = HttpMethod.Put,
                OnFailure = BuildOnFailure(),
                RetryCount = _retryCount,
                Timeout = _timeout,
                Content = input
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
    }
}
