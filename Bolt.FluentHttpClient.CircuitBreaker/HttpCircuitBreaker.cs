using Bolt.FluentHttpClient.Abstracts;
using System;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.CircuitBreaker
{
    public interface IHttpCircuitBreaker
    {
        Task<HttpRequestOutput> SendAsync(string serviceName, HttpRequestInput input);
        Task<HttpRequestOutput> SendAsync<TInput>(string serviceName, HttpRequestInput<TInput> input);
        Task<HttpRequestOutput<TOutput>> SendAsync<TOutput>(string serviceName, HttpRequestInput input);
        Task<HttpRequestOutput<TOutput>> SendAsync<TInput, TOutput>(string serviceName, HttpRequestInput<TInput> input);
    }

    public class HttpCircuitBreakerInput : HttpRequestInput
    {
        public string ServiceName { get; set; }
        public string CircuitKey { get; set; }
        public string AppName { get; set; }
    }

    public class HttpCircuitBreaker : IHttpCircuitBreaker
    {
        private readonly IFluentHttpClient _client;

        public HttpCircuitBreaker(IFluentHttpClient client)
        {
            _client = client;
        }

        public Task<HttpRequestOutput> SendAsync(string serviceName, HttpRequestInput input)
        {
            if (input.Properties == null) input.Properties = new System.Collections.Generic.Dictionary<string, object>();
            input.Properties[PropertyNames.CircuitKey] = input.
            return _client.SendAsync(input);
        }

        public Task<HttpRequestOutput> SendAsync<TInput>(string serviceName, HttpRequestInput<TInput> input)
        {
            throw new NotImplementedException();
        }

        public Task<HttpRequestOutput<TOutput>> SendAsync<TOutput>(string serviceName, HttpRequestInput input)
        {
            throw new NotImplementedException();
        }

        public Task<HttpRequestOutput<TOutput>> SendAsync<TInput, TOutput>(string serviceName, HttpRequestInput<TInput> input)
        {
            throw new NotImplementedException();
        }
    }
}
