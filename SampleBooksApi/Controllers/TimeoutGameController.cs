using Microsoft.AspNetCore.Mvc;
using SampleBooksApi.Contracts;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SampleBooksApi.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class TimeoutGameController : Controller
    {
        public static readonly ConcurrentDictionary<string, int> _score = new ConcurrentDictionary<string, int>();

        [HttpPost("hit")]
        public async Task<TimeoutGameResponse> Hit([FromBody]TimeoutGameInput input)
        {
            var value = _score.GetOrAdd(input.Name, 0);

            Console.WriteLine($"Value {value}");

            if (value < input.DelayCount)
            {
                value++;

                Console.WriteLine($"Value {value}");

                _score.AddOrUpdate(input.Name, value, (s, i) => value);

                Console.WriteLine($"Delaying {input.DelayInMs}");
                await Task.Delay(input.DelayInMs);
            }
            else {
                value++;

                Console.WriteLine($"Value {value}");

                _score.AddOrUpdate(input.Name, value, (s, i) => value);
            }
            return new TimeoutGameResponse
            {
                TotalHit = value
            };
        }

        [HttpDelete("reset")]
        public void Reset()
        {
            _score.Clear();
        }
    }
}
