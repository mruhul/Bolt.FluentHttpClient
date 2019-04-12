using SampleBooksApi.Contracts;
using Shouldly;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentHttpClient_Should : IClassFixture<FluentHttpClientFixture>
    {
        private readonly FluentHttpClientFixture _fixture;

        public FluentHttpClient_Should(FluentHttpClientFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Get_Should_Return_Book()
        {
            var rsp = await _fixture.Request("/api/books/1")
                .GetAsync<Book>();

            var book = rsp.Content;

            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
            book.ShouldNotBeNull();
            book.Id.ShouldBe("1");
            await Reset();
        }

        [Fact]
        public async Task Post_Should_Create_New_Book()
        {
            var rsp = await _fixture.Request("/api/books/")
                    .PostAsync<Book, Book>(new Book {
                        Id = "2",
                        Title = "title 2",
                        Price = 20
                    });


            rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
            rsp.Headers.FirstOrDefault(x => x.Name == "Location").Value.ShouldBe("/api/books/2");
            rsp.Content.ShouldNotBeNull();
            await Reset();
        }

        [Theory]
        [ClassData(typeof(TestCreateBookData))]
        public async Task Post_Should_Read_Error(CreateUser input, HttpStatusCode expectedStatusCode)
        {
            ErrorResponse error = null;
            string statusMessage = string.Empty;
            var rsp = await _fixture.Request("/api/users")
                .OnFailureAsync(HttpStatusCode.InternalServerError, async (msg) => {
                    using (var sr = new StreamReader(msg.Stream))
                    {
                        statusMessage = await sr.ReadToEndAsync();
                    }
                })
                .OnBadRequest<ErrorResponse>(err => {
                    error = err;
                })
                .PostAsync<CreateUser, User>(input);

            rsp.StatusCode.ShouldBe(expectedStatusCode);

            if(rsp.StatusCode == HttpStatusCode.BadRequest)
            {
                error.ShouldNotBeNull();
            }
            else if(rsp.StatusCode == HttpStatusCode.InternalServerError)
            {
                statusMessage.ShouldBe("You asked for trouble");
            }
            else
            {
                rsp.Content.ShouldNotBeNull();
                rsp.Content.Email = input.Email;
            }
        }

        [Fact]
        public async Task Put_Should_Update_Book()
        {
            var rsp = await _fixture.Request("/api/books/2")
                    .PutAsync<Book, Book>(new Book
                    {
                        Id = "2",
                        Title = "title updated 2",
                        Price = 20
                    });


            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
            rsp.Content.ShouldNotBeNull();
            rsp.Content.Title.ShouldBe("title updated 2");
            await Reset();
        }

        [Fact]
        public async Task Delete_Should_Remove_Book()
        {
            var rsp = await _fixture.Request("/api/books/2")
                .DeleteAsync();
            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
            await Reset();
        }

        private async Task Reset()
        {
            var rsp = await _fixture.Request("api/books/reset")
                    .PostAsync();

            rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        public class TestCreateBookData : IEnumerable<object[]>
        {
            private readonly List<object[]> _data = new List<object[]> {

                new object[]
                {
                    null,
                    HttpStatusCode.BadRequest
                },
                new object[]
                {
                    new CreateUser{ Email = "email", Password = "" },
                    HttpStatusCode.BadRequest
                },
                new object[]
                {
                    new CreateUser{ Email = "test@gmail.com", Password = "test", DisplayName = "trouble" },
                    HttpStatusCode.InternalServerError
                },
                new object[]
                {
                    new CreateUser{ Email = "test@gmail.com", Password = "test", DisplayName = "ruhul" },
                    HttpStatusCode.Created
                }
            };

            public IEnumerator<object[]> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
