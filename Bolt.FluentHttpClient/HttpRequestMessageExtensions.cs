using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    internal static class HttpRequestMessageExtensions
    {
        public static T GetPropertyValueOrDefault<T>(this HttpRequestMessage msg, string name)
        {
            object result = null;

            if (msg.Properties?.TryGetValue(name, out result) ?? false)
            {
                return (T)result;
            }

            return default(T);
        }
    }
}
