using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts
{
    public interface IHttpRequestSender
    {
        Task<HttpRequestSenderResponse> SendAsync(HttpRequestSenderInput input, CancellationToken cancellationToken = default);
    }
}
