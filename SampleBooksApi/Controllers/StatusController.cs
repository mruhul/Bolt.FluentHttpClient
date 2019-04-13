using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SampleBooksApi.Controllers
{
    [Route("api/status")]
    public class StatusController : Controller
    {
        private static int count = 0;
        private static object _lock = new object();

        [HttpGet]
        public IActionResult Get([FromQuery]int statusCode)
        {
            var result = Increment();

            return StatusCode(statusCode, result);
        }

        [HttpGet("count")]
        public int Get()
        {
            return count;
        }

        [HttpDelete("reset")]
        public void Reset()
        {
            lock (_lock)
            {
                count = 0;
            }
        }

        private static int Increment()
        {
            lock(_lock)
            {
                count = count + 1;
            }

            return count;
        }
    }
}
