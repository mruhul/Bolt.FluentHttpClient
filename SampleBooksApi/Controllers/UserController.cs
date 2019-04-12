using Microsoft.AspNetCore.Mvc;
using SampleBooksApi.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleBooksApi.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class UsersController : Controller
    {
        private static ConcurrentDictionary<string, User> _store = new ConcurrentDictionary<string, User>();

        [HttpPost]
        public IActionResult Post([FromBody]CreateUser input)
        {
            if(input.DisplayName == "trouble")
            {
                return StatusCode(500, "You asked for trouble");
            }

            var id = Guid.NewGuid().ToString();

            _store[id] = new User {
                Id = id,
                DisplayName = input.DisplayName,
                Email = input.Email,
                PasswordHash = input.Password // don't do this. okay for test purpose
            };

            return Created($"/api/users/{id}", _store[id]);
        }
    }
}
