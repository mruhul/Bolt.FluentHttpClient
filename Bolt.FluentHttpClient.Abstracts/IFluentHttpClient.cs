using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public interface IFluentHttpClient
    {
        Task<HttpRequestOutput> SendAsync(HttpRequestInput input);
        Task<HttpRequestOutput> SendAsync<TInput>(HttpRequestInput<TInput> input);
        Task<HttpRequestOutput<TOutput>> SendAsync<TOutput>(HttpRequestInput input);
        Task<HttpRequestOutput<TOutput>> SendAsync<TInput,TOutput>(HttpRequestInput<TInput> input);
    }

    public class HttpRequestInput
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public IEnumerable<HttpHeader> Headers { get; set; }
        public Dictionary<string,object> Properties { get; set; }
        public int RetryCount { get; set; }
        public TimeSpan Timeout { get; set; }
        public Func<HttpResponseContent, IHttpSerializer, Task> OnFailure { get; set; }
    }

    public class HttpRequestInput<T> : HttpRequestInput
    {
        public string ContentType { get; set; } = "application/json";
        public T Content { get; set; }
    }

    public class HttpRequestOutput
    {
        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<HttpHeader> Headers { get; set; }
    }

    public class HttpRequestOutput<T> : HttpRequestOutput
    {
        public T Content { get; set; }
    }
}
