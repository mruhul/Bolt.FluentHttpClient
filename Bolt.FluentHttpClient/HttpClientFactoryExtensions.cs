using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public static class HttpClientFactoryExtensions
    {
        public static HttpClient CreateDefaultSmartHttpClient(this IHttpClientFactory clientFactory)
        {
            return clientFactory.CreateClient(Constants.HttpClientName);
        }
    }
}
