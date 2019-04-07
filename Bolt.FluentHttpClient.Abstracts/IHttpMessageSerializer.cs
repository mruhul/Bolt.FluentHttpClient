using System;
using System.IO;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public interface IHttpMessageSerializer
    {
        Task Serialize(Stream stream, object value);
        T Deserialize<T>(Stream stream);
    }
}
