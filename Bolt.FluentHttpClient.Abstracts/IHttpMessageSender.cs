using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public interface IHttpMessageSender
    {
        Task<HttpResponseMessage> SendAsync(HttpMessageInput input);
    }
}
