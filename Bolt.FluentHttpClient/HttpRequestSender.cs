using Bolt.FluentHttpClient.Abstracts;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpRequestSender : IHttpRequestSender
    {
        private readonly HttpClient _client;

        public HttpRequestSender(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpRequestSenderResponse> SendAsync(HttpRequestSenderInput input, CancellationToken cancellationToken)
        {
            HttpRequestSenderResponse result = null;

            using (var msg = BuildMessage(input))
            {
                using (var httpResponse = await _client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    result = BuildRequestResponse(httpResponse);

                    var isSucceed = httpResponse.IsSuccessStatusCode;

                    if (isSucceed && input.OnSuccess == null)
                    {
                        HttpRequestLog.Trace("Response succeed and no handler for success defined. so returning basic response");

                        return result;
                    }

                    if (!isSucceed && input.OnFailure == null)
                    {
                        HttpRequestLog.Trace("Failed response and no handler for failure defined. so returning basic response");

                        return result;
                    }

                    if(input.OnSuccess == null && input.OnFailure == null)
                    {
                        HttpRequestLog.Trace("No failure or success action defined so returning basic response");

                        return result;
                    }

                    if (httpResponse.Content == null)
                    {
                        HttpRequestLog.Trace("Content empty so returning basic response");

                        return result;
                    }


                    using (httpResponse.Content)
                    {
                        using (var sr = await httpResponse.Content.ReadAsStreamAsync())
                        {
                            var rsp = new HttpResponseContent
                            {
                                ContentLength = httpResponse.Content.Headers.ContentLength ?? 0,
                                StatusCode = httpResponse.StatusCode,
                                ContentType = httpResponse.Content.Headers.ContentType?.MediaType,
                                Headers = result.Headers,
                                ContentStream = sr
                            };

                            if (isSucceed)
                            {
                                HttpRequestLog.Trace("Executing success handler");
                                await input.OnSuccess(rsp);
                            }
                            else
                            {
                                HttpRequestLog.Trace("Executing failure handler");
                                await input.OnFailure(rsp);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private HttpRequestSenderResponse BuildRequestResponse(HttpResponseMessage msg)
        {
            return new HttpRequestSenderResponse
            {
                StatusCode = msg.StatusCode,
                Headers = msg.Headers?.Select(x => new HttpHeader { Name = x.Key, Value = x.Value == null ? string.Empty : string.Join(",", x.Value) }) ?? Enumerable.Empty<HttpHeader>()
            };
        }


        private HttpRequestMessage BuildMessage(HttpRequestSenderInput input)
        {
            var msg = new HttpRequestMessage();

            msg.Method = input.Method;
            msg.RequestUri = input.Uri;

            if(input.Headers != null)
            {
                foreach(var header in input.Headers)
                {
                    msg.Headers.Add(header.Name, header.Value);
                }
            }

            if (input.Content != null) msg.Content = input.Content;

            if(input.Properties != null)
            {
                foreach(var property in input.Properties)
                {
                    msg.Properties[property.Key] = property.Value;
                }
            }

            if (input.Retry.HasValue)
            {
                msg.Properties[Bolt.FluentHttpClient.Abstracts.Constants.PropertyNameRetryCount] = input.Retry.Value;
            }

            if (input.Timeout.HasValue)
            {
                msg.Properties[Bolt.FluentHttpClient.Abstracts.Constants.PropertyNameTimeout] = input.Timeout.Value;
            }

            return msg;
        }
    }
}
