using Bolt.FluentHttpClient.Abstracts;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient
{
    public class HttpMessageSerializer : IHttpMessageSerializer
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
            using (var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            using (var jw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                new JsonSerializer().Serialize(jw, value);
                await jw.FlushAsync();
            }

            stream.Seek(0, SeekOrigin.Begin);
        }
    }

}
