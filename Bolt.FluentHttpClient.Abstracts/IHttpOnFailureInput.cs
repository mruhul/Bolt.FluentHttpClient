using System.IO;
using System.Net;

namespace Bolt.FluentHttpClient.Abstracts
{
    public interface IHttpOnFailureInput
    {
        HttpStatusCode StatusCode { get; }
        Stream Stream { get; }
        IHttpMessageSerializer Serializer { get; set; }
    }
}
