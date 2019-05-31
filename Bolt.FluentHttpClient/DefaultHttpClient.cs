using Bolt.FluentHttpClient.Abstracts;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class DefaultHttpClient : IFluentHttpClient
    {
        private readonly IHttpRequestSender _sender;
        private readonly IEnumerable<IHttpSerializer> _serializers;

        public DefaultHttpClient(IHttpRequestSender sender, IEnumerable<IHttpSerializer> serializers)
        {
            _sender = sender;
            _serializers = serializers;
        }

        public async Task<HttpRequestOutput> SendAsync(HttpRequestInput input)
        {
            var rsp = await _sender.SendAsync(BuildRequest(input));

            return new HttpRequestOutput
            {
                Headers = rsp.Headers,
                StatusCode = rsp.StatusCode
            };
        }

        public async Task<HttpRequestOutput> SendAsync<TInput>(HttpRequestInput<TInput> input)
        {
            using (var ms = new MemoryStream())
            {
                using (var request = await BuildRequestAsync(input, ms))
                {
                    var rsp = await _sender.SendAsync(request);
                    
                    return new HttpRequestOutput
                    {
                        Headers = rsp.Headers,
                        StatusCode = rsp.StatusCode
                    };
                }
            }
        }

        public async Task<HttpRequestOutput<TOutput>> SendAsync<TInput, TOutput>(HttpRequestInput<TInput> input)
        {
            using (var ms = new MemoryStream())
            {
                using (var request = await BuildRequestAsync(input, ms))
                {
                    TOutput output = default(TOutput);

                    request.OnSuccess = (r) =>
                    {
                        var serializer = _serializers.FirstOrDefault(x => x.IsApplicable(Constants.ContentTypeJson));
                        output = serializer.Deserialize<TOutput>(r.ContentStream);

                        return Task.CompletedTask;
                    };

                    var rsp = await _sender.SendAsync(request);

                    return new HttpRequestOutput<TOutput>
                    {
                        Headers = rsp.Headers,
                        StatusCode = rsp.StatusCode,
                        Content = output
                    };
                }
            }
        }

        public async Task<HttpRequestOutput<TOutput>> SendAsync<TOutput>(HttpRequestInput input)
        {
            using (var request = BuildRequest(input))
            {
                TOutput output = default(TOutput);

                request.OnSuccess = (r) =>
                {
                    var serializer = _serializers.FirstOrDefault(x => x.IsApplicable(Constants.ContentTypeJson));
                    output = serializer.Deserialize<TOutput>(r.ContentStream);

                    return Task.CompletedTask;
                };

                var rsp = await _sender.SendAsync(request);

                return new HttpRequestOutput<TOutput>
                {
                    Headers = rsp.Headers,
                    StatusCode = rsp.StatusCode,
                    Content = output
                };
            }
        }

        private HttpRequestSenderInput BuildRequest(HttpRequestInput input)
        {
            var result = new HttpRequestSenderInput();
            
            result.Url = input.Url;
            result.Headers = input.Headers;
            result.Method = input.Method;
            result.RetryCount = input.RetryCount;
            result.Timeout = input.Timeout;
            result.Properties = input.Properties;

            if(input.OnFailure != null)
            {
                result.OnFailure = (r) => {
                    var serializer = _serializers.FirstOrDefault(x => x.IsApplicable(r.ContentType));
                    return input.OnFailure(r, serializer);
                };
            }

            return result;
        }

        private async Task<HttpRequestSenderInput> BuildRequestAsync<TInput>(HttpRequestInput<TInput> input, Stream stream)
        {
            var result = BuildRequest(input);

            if(input.Content == null)
            {
                result.Content = new StringContent(string.Empty, Encoding.UTF8, input.ContentType ?? Constants.ContentTypeJson);
            }
            else
            {
                var serializer = _serializers.FirstOrDefault(x => x.IsApplicable(input.ContentType));

                await serializer.Serialize(stream, input.Content);

                var content = new StreamContent(stream, 1024);
                content.Headers.ContentLength = stream.Length;
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(input.ContentType ?? Constants.ContentTypeJson) { CharSet = Constants.ContentCharset };

                result.Content = content;
            }

            return result;
        }
    }
}
