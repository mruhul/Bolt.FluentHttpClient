using Bolt.FluentHttpClient.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    public class FluentHttpClientSetupOptions
    {
        public string Name { get; set; } = Constants.HttpClientName;
        public bool EnablePerformanceLog { get; set; } = false;
    }

    public static class IocExtensions
    {
        public static IHttpClientBuilder AddFluentHttpClient(this ServiceCollection sc, FluentHttpClientSetupOptions options = null)
        {
            options = options ?? new FluentHttpClientSetupOptions();
            sc.TryAddSingleton<IHttpMessageSerializer, HttpMessageSerializer>();
            sc.TryAddTransient<IHttpMessageSender>(sp => new HttpMessageSender(sp.GetService<IHttpClientFactory>().CreateClient(options.Name)));
            sc.TryAddTransient<ITypedHttpMessageSender, TypedHttpMessageSender>();
            sc.TryAddTransient<IFluentHttpClient, Fluent.FluentHttpClient>();

            var builder = sc.AddHttpClient(options.Name)
               .ConfigurePrimaryHttpMessageHandler(config => new HttpClientHandler
               {
                   AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                   MaxConnectionsPerServer = 1024
               })
               .AddHttpMessageHandler(() => new HttpClientRetryHandler())
               .AddHttpMessageHandler(() => new HttpClientTimedoutHandler());

            if(options.EnablePerformanceLog)
            {
                builder.AddHttpMessageHandler((sp) => new HttpClientPerfLogHandler(sp.GetRequiredService<ILogger<HttpClientPerfLogHandler>>()));
            }

            return builder;
        }
    }
}
