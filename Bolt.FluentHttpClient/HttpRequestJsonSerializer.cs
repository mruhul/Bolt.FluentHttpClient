using Bolt.FluentHttpClient.Abstracts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpRequestJsonSerializer : IHttpSerializer
    {
        public T Deserialize<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            using (var jr = new JsonTextReader(sr))
            {
                return new JsonSerializer().Deserialize<T>(jr);
            }
        }

        public async Task Serialize(Stream stream, object value)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                new JsonSerializer().Serialize(jw, value);
                await jw.FlushAsync();
            }

            stream.Seek(0, SeekOrigin.Begin);
        }

        public bool IsApplicable(string contentType)
        {
            return string.IsNullOrWhiteSpace(contentType) 
                || string.Equals(contentType, Bolt.FluentHttpClient.Abstracts.Constants.ContentTypeJson, StringComparison.OrdinalIgnoreCase)
                || contentType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
        }

    }
}
