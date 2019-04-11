using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleBooksApi.Contracts;

namespace SampleBooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private static readonly ConcurrentDictionary<string, Book> _store = new ConcurrentDictionary<string, Book>();

        public BooksController()
        {
            SetUp();
        }

        private void SetUp()
        {
            _store.Clear();
            _store.TryAdd("1", new Book
            {
                Id = "1",
                Title = "Pragmatic Programmer",
                Price = 100
            });
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<Book> Get()
        {
            return _store.Values;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Book Get(string id)
        {
            return _store.TryGetValue(id, out var result) ? result : null;
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Book value)
        {
            if (value == null) return BadRequest();

            _store[value.Id] = value;

            return Created($"/api/books/{value.Id}", value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public Book Put(string id, [FromBody] Book value)
        {
            _store[id] = value;

            return value;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _store.TryRemove(id, out var _);
        }

        [HttpPost("reset")]
        public void Reset()
        {
            SetUp();
        }
    }
}
