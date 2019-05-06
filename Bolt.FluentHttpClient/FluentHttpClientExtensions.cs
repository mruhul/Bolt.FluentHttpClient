using Bolt.FluentHttpClient.Abstracts;

namespace Bolt.FluentHttpClient
{
    public static class FluentHttpClientExtensions
    {
        public static Fluent.IHaveUrl ForUrl(this IFluentHttpClient http, string url)
        {
            return new Fluent.FluentHttpClientImp(http).Url(url);
        }
    }
}
