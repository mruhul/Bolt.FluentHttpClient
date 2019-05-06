using System.IO;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public interface IHttpSerializer
    {
        Task Serialize(Stream stream, object value);
        T Deserialize<T>(Stream stream);
        bool IsApplicable(string contentType);
    }
}
