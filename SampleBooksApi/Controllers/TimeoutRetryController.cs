using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleBooksApi.Controllers
{
    [Route("api/timeout-test")]
    public class TimeoutRetryController : Controller
    {
        private static int _retry;

        [HttpGet]
        public async Task<IActionResult> Retry(int timeoutMs, int retry)
        {
            if (_retry == retry) return Ok(_retry);

            _retry++;

            await Task.Delay(timeoutMs);

            return Ok(-1);
        }
    }
}
