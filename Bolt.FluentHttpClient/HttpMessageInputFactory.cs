using Bolt.FluentHttpClient.Abstracts;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Bolt.FluentHttpClient
{
    internal static class HttpMessageInputFactory
    {
        public static HttpMessageInput Create(TypedHttpMessageInput input)
        {
            var result = new HttpMessageInput();

            result.Method = input.Method;
            result.Headers = input.Headers;
            result.RetryCount = input.RetryCount;
            result.Timeout = input.Timeout;
            result.Url = input.Url;

            return result;
        }

        public static HttpMessageInput Create<TContent>(TypedHttpMessageInput<TContent> input, Stream stream, IHttpMessageSerializer serializer)
        {
            var result = new HttpMessageInput();

            result.Method = input.Method;
            result.Headers = input.Headers;
            result.RetryCount = input.RetryCount;
            result.Timeout = input.Timeout;
            result.Url = input.Url;

            if (input.Content == null)
            {
                result.Content = new StringContent(string.Empty, Encoding.UTF8, input.ContentType ?? Constants.ContentTypeJson);
                return result;
            };

            serializer.Serialize(stream, input.Content);

            result.Content = new StreamContent(stream, 1024);
            result.Content.Headers.ContentLength = stream.Length;
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(input.ContentType ?? Constants.ContentTypeJson) { CharSet = Constants.ContentCharset };

            return result;
        }
    }
}
