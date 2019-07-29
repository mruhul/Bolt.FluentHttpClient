using Bolt.FluentHttpClient.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public class FluentHttpClientSetupOptions
    {
        public string Name { get; set; } = Constants.HttpClientName;
        public bool EnablePerformanceLog { get; set; } = false;
    }

    public static class IocSetup
    {
        public static IHttpClientBuilder AddFluentHttpClient(this IServiceCollection sc, FluentHttpClientSetupOptions options = null)
        {
            options = options ?? new FluentHttpClientSetupOptions();

            sc.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpSerializer, HttpRequestJsonSerializer>());
            sc.TryAddTransient<IHttpRequestSender>(sp => new HttpRequestSender(sp.GetService<IHttpClientFactory>().CreateClient(options.Name)));
            sc.TryAddTransient<IFluentHttpClient, DefaultHttpClient>();

            var builder = sc.AddHttpClient(options.Name)
               .ConfigurePrimaryHttpMessageHandler(config => new HttpClientHandler
               {
                   AllowAutoRedirect = false,
                   UseCookies = false,
                   AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                   MaxConnectionsPerServer = 1024,
               })
               .AddHttpMessageHandler(() => new HttpClientRetryHandler())
               .AddHttpMessageHandler(() => new HttpClientTimedoutHandler());

            if (options.EnablePerformanceLog)
            {
                builder.AddHttpMessageHandler((sp) => new HttpClientPerfLogHandler());
            }

            return builder;
        }
    }
}
