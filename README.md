# Bolt.FluentHttpClient

A library to use httpclient fluently and support for Timeout / Retry.
- Support timeout
- Support Retry
- Support strong type data output and input using json serialization
- Support read of different Error dto based on failure status codes

## How to use this library

Add nuget reference `Bolt.FluentHttpClient` in your project. Register the library with your Ioc in startup configure method as below:

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
                .Url($"http://api-books.bookwork.com.au/books/{id}")
                .Retry(1)
                .TimeoutInMilliseconds(500)
                .GetAsync<BookDto>();
            
            return response.Content;
        }

		public async Task<BookDto> Create(CreateBookInput input)
        {
            var resonse = await _client
                .Url("http://api-books.bookwork.com.au/books/")
                .TimeoutInMilliseconds(500)
                .PostAsync<CreateBookInput,BookDto>(input);
                // If you don't expect any response content then use following
                // .PostAsync<CreateBookInput>(input);
                // if you don't have any input but expect output then use following
                // .PostAsync<BookDto>();
				
            return response.Content;
        }
    }

When doing post or put if you don't have input but expect some response content then use following

    ...
    .PostAsync<TOutput>();

    // for put
    ...
    .PutAsync<TOutput>();

When doing post/put if you don't expect any output but has input then use following

    ...
    .PostAsync(TInput);

    // for put
    ...
    .PutAsync(TInput);


## How to read content when the status code is not successful.

For example the endpoint you requesting can return badrequest and you like to get Errros when this happen. First define the Error dto and then use that dto as below:

    Error err = null;
    AuthError authError = null;
    var rsp = await _fluentHttpClient
        .Url("http://api-books.com.au/books/")
        .OnBadRequest<Error>(dto => { err = dto; })
        .OnFailure<AuthError>(HttpStatusCode.Unauthorized, dto => { authError = dto; })
        .PostAsync<CreateBook>(new CreateBook
        { 
            ...
        });

    if(rsp.StatusCode == HttpStatusCode.OK) return rsp.Content;
    if(rsp.StatusCode == HttpStatusCode.BadRequest) return err;
    if(rsp.StatusCode == HttpStatusCode.Unauthorized) return authErr; // just to show. will not compile :)



## You can enable each requests log with time taken, statuscode

Just update your startup configure code as below:

    serviceCollection.AddFluentHttpClient(new FluentHttpClientSetupOptions { EnablePerformanceLog = true });

