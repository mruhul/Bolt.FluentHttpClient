using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Bolt.FluentHttpClient.Abstracts
{
    public class HttpMessageInput : IDisposable
    {
        public string Url { get; set; }
        public List<HttpHeader> Headers { get; set; } = new List<HttpHeader>();
        public TimeSpan Timeout { get; set; }
        public int RetryCount { get; set; }
        public HttpMethod Method { get; set; }
        public HttpContent Content { get; set; }

        public void Dispose()
        {
            Content?.Dispose();
        }
    }
}
