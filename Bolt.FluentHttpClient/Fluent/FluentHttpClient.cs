using Bolt.FluentHttpClient.Abstracts;
using Bolt.FluentHttpClient.Abstracts.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public class FluentHttpClient : IFluentHttpClient, 
        IHaveUrl, IHaveOnFailure, IHaveStatusBasedOnFailure, IHaveOnBadRequest, IHaveHeaders, IHaveRetryCount, IHaveTimeout, ISendMessage
    {
        private readonly ITypedHttpMessageSender _sender;

        private TimeSpan _timeout = TimeSpan.Zero;
        private int _retry = 0;
        private string _url;
        private readonly List<HttpHeader> _headers = new List<HttpHeader>();
        private Dictionary<HttpStatusCode, Func<IHttpOnFailureInput,Task>> _statusBasedFailureHandlers;
        private Func<IHttpOnFailureInput,Task> _genericFailureHandler;

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
            var msg = BuildInput<TypedHttpMessageInput>(HttpMethod.Post);

            return _sender.SendAsync<TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> PostAsync<TInput, TOutput>(TInput input)
        {
            var msg = BuildInput<TypedHttpMessageInput<TInput>>(HttpMethod.Post);
            msg.Content = input;
            msg.ContentType = Constants.ContentTypeJson;

            return _sender.SendAsync<TInput, TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput> PostAsync()
        {
            var msg = BuildInput<TypedHttpMessageInput>(HttpMethod.Post);

            return _sender.SendAsync(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> PutAsync<TOutput>()
        {
            var msg = BuildInput<TypedHttpMessageInput>(HttpMethod.Put);

            return _sender.SendAsync<TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> PutAsync<TInput, TOutput>(TInput input)
        {
            var msg = BuildInput<TypedHttpMessageInput<TInput>>(HttpMethod.Put);

            msg.Content = input;
            msg.ContentType = Constants.ContentTypeJson;

            return _sender.SendAsync<TInput, TOutput>(msg);
        }

        public Task<TypedHttpMessageOutput> PutAsync()
        {
            var msg = BuildInput<TypedHttpMessageInput>(HttpMethod.Put);

            return _sender.SendAsync(msg);
        }

        public Task<TypedHttpMessageOutput> DeleteAsync()
        {
            var msg = BuildInput<TypedHttpMessageInput>(HttpMethod.Delete);

            return _sender.SendAsync(msg);
        }

        public Task<TypedHttpMessageOutput<TOutput>> GetAsync<TOutput>()
        {
            var msg = BuildInput<TypedHttpMessageInput>(HttpMethod.Get);

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

        public IHaveOnFailure OnFailureAsync(Func<IHttpOnFailureInput,Task> handlerAsync)
        {
            _genericFailureHandler = handlerAsync;

            return this;
        }

        public IHaveStatusBasedOnFailure OnFailureAsync(HttpStatusCode statusCode, Func<IHttpOnFailureInput,Task> handlerAsync)
        {
            if (_statusBasedFailureHandlers == null) _statusBasedFailureHandlers = new Dictionary<HttpStatusCode, Func<IHttpOnFailureInput,Task>>();
            _statusBasedFailureHandlers[statusCode] = handlerAsync;
            return this;
        }

        public IHaveStatusBasedOnFailure OnFailure<T>(HttpStatusCode statusCode, Action<T> handler)
        {
            if (_statusBasedFailureHandlers == null) _statusBasedFailureHandlers = new Dictionary<HttpStatusCode, Func<IHttpOnFailureInput,Task>>();
            _statusBasedFailureHandlers[statusCode] = new Func<IHttpOnFailureInput,Task>((input) => 
            {
                handler(input.Serializer.Deserialize<T>(input.Stream));
                return Task.CompletedTask;
            });

            return this;
        }

        public IHaveOnBadRequest OnBadRequest<T>(Action<T> handler)
        {
            OnFailure(HttpStatusCode.BadRequest, handler);

            return this;
        }

        private TInput BuildInput<TInput>(HttpMethod method) where TInput : TypedHttpMessageInput, new()
        {
            var result = new TInput
            {
                Headers = _headers,
                Method = method,
                RetryCount = _retry,
                Timeout = _timeout,
                Url = _url,
                StatusCodesToHandleFailure = _statusBasedFailureHandlers?.Keys
            };

            if(_genericFailureHandler != null)
            {
                result.onFailureAsync = _genericFailureHandler;
            }
            else if(_statusBasedFailureHandlers?.Count > 0)
            {
                result.onFailureAsync = new Func<IHttpOnFailureInput, Task>((input) =>
                {
                    foreach(var keyValue in _statusBasedFailureHandlers)
                    {
                        if(keyValue.Key == input.StatusCode)
                        {
                            return keyValue.Value(input);
                        }
                    }

                    return Task.CompletedTask;
                });
            }

            return result;
        }
    }
}
