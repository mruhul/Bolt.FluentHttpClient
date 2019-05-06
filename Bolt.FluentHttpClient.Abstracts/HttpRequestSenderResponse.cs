using System.Collections.Generic;
using System.Net;

namespace Bolt.FluentHttpClient.Abstracts
{
    public class HttpRequestSenderResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public IEnumerable<HttpHeader> Headers { get; set; }

    }
}
