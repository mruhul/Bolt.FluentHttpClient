using Bolt.FluentHttpClient.Abstracts;
using Bolt.FluentHttpClient.Abstracts.Fluent;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public class FluentHttpClient : IFluentHttpClient, IHaveUrl, IHaveHeaders, IHaveRetryCount, IHaveTimeout, ISendMessage
    {
        private readonly ITypedHttpMessageSender _sender;

        private TimeSpan _timeout = TimeSpan.Zero;
        private int _retry = 0;
        private string _url;
        private readonly List<HttpHeader> _headers = new List<HttpHeader>();

        public FluentHttpClient(ITypedHttpMessageSender sender)
        {
            _sender = sender;
        }

        public IHaveHeaders Header(params HttpHeader[] headers)
        {
            _headers.AddRange(headers);
            return this;
        }

        public IHaveHeaders Header(HttpHeader header)
        {
            _headers.Add(header);
            return this;
        }

        public IHaveHeaders Header(string name, string value)
        {
            _headers.Add(new HttpHeader { Name = name, Value = value });
            return this;
        }

        public IHaveHeaders Header(IEnumerable<HttpHeader> headers)
        {
            _headers.AddRange(headers);
            return this;
        }

        public IHaveUrl Url(string url)
        {
            _url = url;

            return this;
        }

        public Task<TypedHttpMessageOutput<TOutput>> PostAsync<TOutput>()
        {
            var msg = new TypedHttpMessageInput
            {
                Headers = _headers,
                Method = HttpMethod.Post,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url
            };

            return _sender.SendAsync<TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> PostAsync<TInput, TOutput>(TInput input)
        {
            var msg = new TypedHttpMessageInput<TInput>
            {
                Headers = _headers,
                Method = HttpMethod.Post,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url,
                Content = input,
                ContentType = Constants.ContentTypeJson
            };

            return _sender.SendAsync<TInput, TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput> PostAsync()
        {
            var msg = new TypedHttpMessageInput
            {
                Headers = _headers,
                Method = HttpMethod.Post,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url
            };

            return _sender.SendAsync(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> PutAsync<TOutput>()
        {
            var msg = new TypedHttpMessageInput
            {
                Headers = _headers,
                Method = HttpMethod.Put,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url
            };

            return _sender.SendAsync<TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> PutAsync<TInput, TOutput>(TInput input)
        {
            var msg = new TypedHttpMessageInput<TInput>
            {
                Headers = _headers,
                Method = HttpMethod.Put,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url,
                Content = input,
                ContentType = Constants.ContentTypeJson
            };

            return _sender.SendAsync<TInput, TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput> PutAsync()
        {
            var msg = new TypedHttpMessageInput
            {
                Headers = _headers,
                Method = HttpMethod.Put,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url
            };

            return _sender.SendAsync(msg);
        }

        public Task<TypedHttpMessageOutput> DeleteAsync()
        {
            var msg = new TypedHttpMessageInput
            {
                Headers = _headers,
                Method = HttpMethod.Delete,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url
            };

            return _sender.SendAsync(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> GetAsync<TOutput>()
        {
            var msg = new TypedHttpMessageInput
            {
                Headers = _headers,
                Method = HttpMethod.Get,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url
            };

            return _sender.SendAsync<TOutput>(msg);
        }

        public IHaveRetryCount Retry(int count)
        {
            _retry = count;
            return this;
        }

        public IHaveTimeout Timeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        public IHaveTimeout TimeoutInMilliseconds(int ms)
        {
            _timeout = TimeSpan.FromMilliseconds(ms);
            return this;
        }

        public IHaveTimeout TimeoutInMinutes(int minutes)
        {
            _timeout = TimeSpan.FromMinutes(minutes);
            return this;
        }

        public IHaveTimeout TimeoutInSeconds(int seconds)
        {
            _timeout = TimeSpan.FromSeconds(seconds);
            return this;
        }
    }
}
