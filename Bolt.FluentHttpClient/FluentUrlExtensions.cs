namespace Bolt.FluentHttpClient
{
    public static class FluentUrlExtensions
    {
        public static Fluent.ICollectFluentUrlPath ToFluentUrl(this string path)
        {
            return Fluent.FluentUrl.FromUrl(path);
        }
    }
}
