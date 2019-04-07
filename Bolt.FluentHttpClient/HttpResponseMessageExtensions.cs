using Bolt.FluentHttpClient.Abstracts;
using System.Collections.Generic;
using System.Net.Http;

namespace Bolt.FluentHttpClient
{
    internal static class HttpResponseMessageExtensions
    {
        public static List<HttpHeader> GetHeaders(this HttpResponseMessage source)
        {
            var result = new List<HttpHeader>();

            if (source.Headers == null) return result;

            foreach(var item in source.Headers)
            {
                result.Add(new HttpHeader
                {
                    Name = item.Key,
                    Value = item.Value == null ? string.Empty : string.Join(",", item.Value)
                });
            }

            return result;
        }
    }
}
