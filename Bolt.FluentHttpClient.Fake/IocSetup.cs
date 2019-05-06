using Bolt.FluentHttpClient.Fake;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.FluentHttpClient
{
    public static class IocSetup
    {
        public static IHttpClientBuilder AddFakeFluentHttpClient(this IServiceCollection sc, string httpClientName)
        {
            sc.TryAddEnumerable(ServiceDescriptor.Transient<IFakeResponse, LocalFileBasedFakeResponse>());
            sc.TryAddTransient<IFakeResponseStore, FileBasedFakeResponseStore>();
            sc.TryAddTransient<HttpClientFakeHandler>();

            return sc.AddHttpClient(httpClientName)
                .AddHttpMessageHandler<HttpClientFakeHandler>();
        }
    }
}
