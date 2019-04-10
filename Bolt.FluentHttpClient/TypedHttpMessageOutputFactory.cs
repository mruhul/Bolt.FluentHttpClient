using Bolt.FluentHttpClient.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    internal class TypedHttpMessageOutputFactory
    {
        public static async Task<TypedHttpMessageOutput> Create(HttpResponseMessage rsp, IHttpMessageSerializer serializer, IEnumerable<HttpStatusCode> erroCodesToHandle, Action<IHttpOnFailureInput> onFailure)
        {
            using (rsp)
            {
                var result = new TypedHttpMessageOutput();

                result.StatusCode = rsp.StatusCode;
                result.Headers = rsp.GetHeaders();

                if (rsp.IsSuccessStatusCode) return result;

                if (rsp.Content != null && onFailure != null)
                {
                    await HandleFailure(rsp, serializer, erroCodesToHandle, onFailure);
                }

                return result;
            }
        }

        public static async Task<TypedHttpMessageOutput<TContent>> Create<TContent>(HttpResponseMessage rsp, IHttpMessageSerializer serializer, IEnumerable<HttpStatusCode> errorStatusCodesToHandle, Action<IHttpOnFailureInput> onFailure)
        {
            using (rsp)
            {
                var result = new TypedHttpMessageOutput<TContent>();

                result.StatusCode = rsp.StatusCode;
                result.Headers = rsp.GetHeaders();

                if (rsp.IsSuccessStatusCode)
                {
                    if(rsp.Content != null)
                    {
                        using(var cnt = rsp.Content)
                        {
                            using (var sr = await cnt.ReadAsStreamAsync())
                            {
                                result.Content = serializer.Deserialize<TContent>(sr);
                            }
                        }
                    }

                    return result;
                }

                if (rsp.Content != null && onFailure != null)
                {
                    await HandleFailure(rsp, serializer, errorStatusCodesToHandle, onFailure);
                }

                return result;
            }
        }

        private static async Task HandleFailure(HttpResponseMessage rsp, IHttpMessageSerializer serializer, IEnumerable<HttpStatusCode> erroCodesToHandle, Action<IHttpOnFailureInput> onFailure)
        {
            if(erroCodesToHandle == null)
            {
                if (rsp.StatusCode != HttpStatusCode.BadRequest) return;
            }
            else
            {
                if(!erroCodesToHandle.Any(x => x == rsp.StatusCode))
                {
                    return;
                }
            }

            using (rsp.Content)
            {
                using (var stream = await rsp.Content.ReadAsStreamAsync())
                {
                    onFailure(new TypedHttpOnFailureInput
                    {
                        Serializer = serializer,
                        Stream = stream,
                        StatusCode = rsp.StatusCode
                    });
                }
            }
        }
    }
}
