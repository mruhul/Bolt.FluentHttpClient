using Bolt.Common.Extensions;
using Newtonsoft.Json;
using SampleBooksApi.Contracts;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bolt.FluentHttpClient.Tests
{
    public class UnitTest1 : IClassFixture<FluentHttpClientFixture>
    {
        private readonly FluentHttpClientFixture _fixture;

        public UnitTest1(FluentHttpClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Test1()
        {
            await _fixture.Client.Url("http://dummy.restapiexample.com/api/v1/employees")
                    .GetAsync<dynamic>();

            var response = await _fixture.Request("/api/books/")
                .GetAsync<IEnumerable<Book>>();

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            //response.Content.ShouldBeEmpty();

            var client = _fixture.GetRequiredService<IHttpClientFactory>().CreateClient("test");
            var bookinput = new Book { Id = "2", Title = "3", Price = 100 };
            var s = await client.PostAsync("http://localhost:50276/api/books", new StringContent(JsonConvert.SerializeObject(bookinput, Formatting.None), Encoding.UTF8, "application/json"));

            s.StatusCode.ShouldBe(HttpStatusCode.Created);

            var addResponse = await _fixture
                .Client
                .Url("http://localhost:50276/api/books")
                .Retry(1)
                .TimeoutInMilliseconds(500)
                .PostAsync<Book, Book>(bookinput);

            addResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            addResponse.Headers.FirstOrDefault(x => x.Name.IsSame("location")).Value.ShouldBe($"/api/books/{addResponse.Content.Id}");
        }
    }
}
