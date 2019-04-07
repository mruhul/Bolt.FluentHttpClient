using Bolt.FluentHttpClient.Abstracts;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    internal class TypedHttpMessageOutputFactory
    {
        public static async Task<TypedHttpMessageOutput> Create(HttpResponseMessage rsp)
        {
            using (rsp)
            {
                var result = new TypedHttpMessageOutput();

                result.StatusCode = rsp.StatusCode;
                result.Headers = rsp.GetHeaders();

                if (rsp.IsSuccessStatusCode) return result;

                using (rsp.Content)
                {
                    result.ErrorContent = await rsp.Content?.ReadAsStringAsync();
                }

                return result;
            }
        }

        public static async Task<TypedHttpMessageOutput<TContent>> Create<TContent>(HttpResponseMessage rsp, IHttpMessageSerializer serializer)
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

                                await sr.FlushAsync();
                            }
                        }
                    }

                    return result;
                }

                if (rsp.Content != null)
                {
                    using (rsp.Content)
                    {
                        result.ErrorContent = await rsp.Content.ReadAsStringAsync();
                    }
                }

                return result;
            }
        }
    }
}
