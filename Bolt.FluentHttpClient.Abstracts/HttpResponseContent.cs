using System.IO;

namespace Bolt.FluentHttpClient.Abstracts
{
    public class HttpResponseContent : HttpRequestSenderResponse
    {
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public Stream ContentStream { get; set; }
    }
}
