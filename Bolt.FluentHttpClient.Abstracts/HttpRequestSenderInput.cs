using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public class HttpRequestSenderInput : IDisposable
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public IEnumerable<HttpHeader> Headers { get; set; }
        public HttpContent Content { get; set; }

        public int RetryCount { get; set; }
        public TimeSpan Timeout { get; set; }

        public Func<HttpResponseContent,Task> OnSuccess { get; set; }
        public Func<HttpResponseContent, Task> OnFailure { get; set; }

        public void Dispose()
        {
            Content?.Dispose();
        }
    }
}
