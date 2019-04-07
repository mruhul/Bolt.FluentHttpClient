# Bolt.FluentHttpClient

A library to use httpclient fluently.

## How to use this library

Add nuget reference `Bolt.FluentHttpClient` in your project. Rregister the library with your Ioc in startup configure method as below:

    serviceCollection.AddFluentHttpClient();

inject `IFluentHttpClient` in your class and sample usage below:

    public class BooksService
    {
        private readonly IFluentHttpClient _client;

        public BooksService(IFluentHttpClient client)
        {
            _client = client;
        }

        public async Task<BookDto> GetById(string id)
        {
            var resonse = await _client
                .Path($"http://api-books.bookwork.com.au/books/{id}")
                .Retry(1)
                .TimeoutInMilliseconds(500)
                .GetAsync<BookDto>();
            
            return response.Content;
        }
    }
