# Bolt.FluentHttpClient

A library to use httpclient fluently.

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
    var rsp = await _fluentHttpClient
        .Url("http://api-books.com.au/books/")
        .OnBadRequest<Error>(dto => { err = dto; })
        .PostAsync<CreateBook>(new CreateBook
        { 
            ...
        });

    if(rsp.StatusCode == HttpStatusCode.OK) return rsp.Content;
    if(rsp.StatusCode == HttpStatusCode.BadRequest) return err; // just to show. will not compile :)

You can even do this for any status code. e.g

    .OnFailure<AuthError>(HttpStatusCode.Unauthorized, authErr => { // assign authErr to you local variable... });

## You can enable each requests log with time taken, statuscode

Just update your startup configure code as below:

    serviceCollection.AddFluentHttpClient(new FluentHttpClientSetupOptions { EnablePerformanceLog = true });

