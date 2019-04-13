using SampleBooksApi.Contracts;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bolt.FluentHttpClient.Tests
{
    public class When_Retry_Set_FluentClient_Should : IClassFixture<FluentHttpClientFixture>
    {
        private readonly FluentHttpClientFixture fixture;

        public When_Retry_Set_FluentClient_Should(FluentHttpClientFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Retry_On_Timeout()
        {
            await fixture.Request("api/game/reset")
                .DeleteAsync();
            var response = await fixture.Request("api/game/hit")
                .Retry(2)
                .TimeoutInMilliseconds(1000 - 10)
                .PostAsync<TimeoutGameInput, TimeoutGameResponse>(new TimeoutGameInput {
                    DelayCount = 2,
                    DelayInMs = 1000,
                    Name = "test"
                });

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
            response.Content.TotalHit.ShouldBe(3);

        }

        [Theory]
        [InlineData(400, 1)] // shouldn't retry on notfound
        [InlineData(404, 1)] // shouldn't retry on bad request
        [InlineData(500, 3)] // should retry on internal server error
        [InlineData(408, 3)] // should retry on request timeout
        [InlineData(502, 3)] // should retry on bad gatway
        [InlineData(503, 3)] // should retry on service unavailable
        public async Task Retry_On_Failed(int status, int expectedCount)
        {
            await fixture.Request("/api/status/reset")
                .DeleteAsync();

            var count = 0;
            var rsp = await fixture.Request($"/api/status?statusCode={status}")
                .Retry(2)
                .OnFailure<int>((HttpStatusCode)status, r => { count = r; })   
                .GetAsync<int>();

            rsp.StatusCode.ShouldBe((HttpStatusCode)status);
            count.ShouldBe(expectedCount);
        }
    }
}
