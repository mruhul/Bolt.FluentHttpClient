using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fake
{
    public interface IResponseReceivedListener
    {
        Task Notify(HttpRequestMessage req, HttpResponseMessage rsp);
    }
}
