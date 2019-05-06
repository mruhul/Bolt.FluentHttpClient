using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fake
{
    public interface IFakeResponse
    {
        Task<HttpResponseMessage> Get(HttpRequestMessage msg);
    }
}
