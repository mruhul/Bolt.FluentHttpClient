using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SampleBooksApi.Contracts;
using Xunit;

namespace Bolt.FluentHttpClient.Tests
{
    public class Fake_Request_Should : IClassFixture<FluentHttpClientFixture>
    {
        private readonly FluentHttpClientFixture _fixture;

        public Fake_Request_Should(FluentHttpClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Return_Collection()
        {
            var rsp = await _fixture.Client.ForUrl("http://nodomain/test1/books")
                            .GetAsync<ICollection<Book>>();

            rsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Return_Single_Item()
        {
            var rsp = await _fixture.Client.ForUrl("http://nodomain/test2/books/100")
                            .GetAsync<Book>();

            rsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            rsp.Content.Id.ShouldBe("100");
            rsp.Content.Price.ShouldBe(99.99);
        }
    }
}
