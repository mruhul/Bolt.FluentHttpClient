using Bolt.FluentHttpClient.Abstracts;
using Bolt.FluentHttpClient.Abstracts.Fluent;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentHttpClientFixture
    {
        private IServiceProvider _sp;

        public FluentHttpClientFixture()
        {
            var sc = new ServiceCollection();
            sc.AddFluentHttpClient(new FluentHttpClientSetupOptions { EnablePerformanceLog = false });
            _sp = sc.BuildServiceProvider();
        }

        public TService GetRequiredService<TService>()
        {
            return _sp.GetRequiredService<TService>();
        }

        public IFluentHttpClient Client => _sp.GetRequiredService<IFluentHttpClient>();
        public IHaveUrl Request(string path) => _sp.GetRequiredService<IFluentHttpClient>().Url($"http://localhost:58252/{path.TrimStart('/')}");
    }
}
