using System.IO;
using System.Net;

namespace Bolt.FluentHttpClient.Abstracts
{
    public class HttpResponseContent : HttpRequestSenderResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public Stream ContentStream { get; set; }
    }
}
