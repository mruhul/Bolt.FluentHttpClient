using Bolt.FluentHttpClient.Abstracts;
using Bolt.FluentHttpClient.Fluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.FluentHttpClient.CircuitBreaker
{
    public static class CircuitBreakerHttpExtensions
    {
        public static IHttpFluentCircuit WithCircuitBreaker(this IFluentHttpClient client, string circuitKey)
        {
            return new HttpFluentCircuit(client, circuitKey);
        }
    }

    public interface IHttpFluentCircuit
    {
        Fluent.IHaveProperties Url(string url);
    }

    internal class HttpFluentCircuit : IHttpFluentCircuit
    {
        private readonly IFluentHttpClient _client;
        private readonly string _circuitKey;

        public HttpFluentCircuit(IFluentHttpClient client, string circuitKey)
        {
            _client = client;
            _circuitKey = circuitKey;
        }

        public IHaveProperties Url(string url)
        {
            return _client.ForUrl(url)
                .Property("_cb:circuitkey", _circuitKey);
        }
    }
}
