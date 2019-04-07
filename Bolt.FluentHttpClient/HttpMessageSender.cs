using Bolt.FluentHttpClient.Abstracts;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpMessageSender : IHttpMessageSender
    {
        private readonly HttpClient _client;

        public HttpMessageSender(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMessageInput input)
        {
            using (var msg = BuildRequestMessage(input))
            {
                using (msg.Content)
                {
                    return await _client.SendAsync(msg);
                }
            }
        }

        private static HttpRequestMessage BuildRequestMessage(HttpMessageInput input)
        {
            var msg = new HttpRequestMessage();

            msg.Properties[Constants.PropertyNameRetryCount] = input.RetryCount;
            msg.Properties[Constants.PropertyNameTimeout] = input.Timeout;

            msg.RequestUri = new Uri(input.Url);
            msg.Method = input.Method;

            if (input.Headers != null)
            {
                foreach (var item in input.Headers)
                {
                    msg.Headers.Add(item.Name, item.Value);
                }
            }

            msg.Content = input.Content;

            return msg;
        }
    }
}
