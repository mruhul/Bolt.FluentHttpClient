using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public class TypedHttpMessageInput
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public int RetryCount { get; set; }
        public TimeSpan Timeout { get; set; }
        public List<HttpHeader> Headers { get; set; } = new List<HttpHeader>();
    }

    public class TypedHttpMessageInput<TContent> : TypedHttpMessageInput
    {
        private const string DefaultContentType = "application/json";
        public string ContentType { get; set; } = DefaultContentType;
        public TContent Content { get; set; }
    }

    public class TypedHttpMessageOutput
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<HttpHeader> Headers { get; set; } = new List<HttpHeader>();
        public string ErrorContent { get; set; }
    }

    public class TypedHttpMessageOutput<TContent> : TypedHttpMessageOutput
    {
        public TContent Content { get; set; }
    }

    public interface ITypedHttpMessageSender
    {
        Task<TypedHttpMessageOutput> SendAsync(TypedHttpMessageInput input);
        Task<TypedHttpMessageOutput<TOutput>> SendAsync<TOutput>(TypedHttpMessageInput input);
        Task<TypedHttpMessageOutput> SendAsync<TInput>(TypedHttpMessageInput<TInput> input);
        Task<TypedHttpMessageOutput<TOutput>> SendAsync<TInput,TOutput>(TypedHttpMessageInput<TInput> input);
    }
}
