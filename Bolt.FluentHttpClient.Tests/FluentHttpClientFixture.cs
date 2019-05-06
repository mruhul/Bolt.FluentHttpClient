using Bolt.FluentHttpClient.Abstracts;
using Bolt.FluentHttpClient.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Net.Http;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentHttpClientFixture
    {
        private IServiceProvider _sp;

        public FluentHttpClientFixture()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var sc = new ServiceCollection();
            var name = "httprequest.test";
            sc.AddFakeFluentHttpClient(name);
            sc.AddFluentHttpClient(new FluentHttpClientSetupOptions { Name = name, EnablePerformanceLog = true });

            sc.AddLogging(lb => lb.AddSerilog(dispose: true));
            sc.AddHttpClient("raw")
                .ConfigurePrimaryHttpMessageHandler(h => new HttpClientHandler
                {

                    AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip,
                    MaxConnectionsPerServer = 1024,
                });
            _sp = sc.BuildServiceProvider();
        }

        public TService GetRequiredService<TService>()
        {
            return _sp.GetRequiredService<TService>();
        }

        public IFluentHttpClient Client => _sp.GetRequiredService<IFluentHttpClient>();
        public IHaveUrl Request(string path) => _sp.GetRequiredService<IFluentHttpClient>().ForUrl($"http://localhost:50276/{path.TrimStart('/')}");
    }
}
