using Bolt.FluentHttpClient.Abstracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Fluent
{
    public interface IHaveUrl : ICollectHeader, ICollectTimeout, ICollectRetryCount, ICollectOnFailure, IRequestExecute
    {
    }
    public interface ICollectUrl
    {
        IHaveUrl Url(string url);
    }

    public interface IHaveHeader : ICollectHeader, ICollectRetryCount, ICollectTimeout, ICollectOnFailure, IRequestExecute { }
    public interface ICollectHeader
    {
        IHaveHeader Header(HttpHeader header);
        IHaveHeader Headers(IEnumerable<HttpHeader> headers);
        IHaveHeader Header(string name, string value);
    }

    public interface IHaveRetryCount : IRequestExecute, ICollectOnFailure, ICollectTimeout { }
    public interface ICollectRetryCount
    {
        IHaveRetryCount Retry(int retry);
    }

    public interface IHaveTimeout : IRequestExecute, ICollectOnFailure, ICollectRetryCount { }
    public interface ICollectTimeout
    {
        IHaveTimeout Timeout(TimeSpan timespan);
        IHaveTimeout TimeoutInMilliseconds(int milliseconds);
    }

    public interface IHaveOnFailure : ICollectOnFailure, IRequestExecute { }
    public interface ICollectOnFailure
    {
        IHaveOnFailure OnFailure(Func<HttpResponseContent, IHttpSerializer, Task> onFailureAsync);
        IHaveOnFailure OnFailure(HttpStatusCode statusCode, Func<HttpResponseContent, IHttpSerializer, Task> onFailureAsync);
        IHaveOnFailure OnFailure<TError>(HttpStatusCode statusCode, Action<TError> onBadRequest);
        IHaveOnFailure OnBadRequest<TError>(Action<TError> onBadRequest);
    }

    public interface IRequestExecute
    {
        Task<HttpRequestOutput<T>> GetAsync<T>();
        Task<HttpRequestOutput> PostAsync();
        Task<HttpRequestOutput<TOutput>> PostAsync<TOutput>();
        Task<HttpRequestOutput> PostAsync<TInput>(TInput input);
        Task<HttpRequestOutput<TOutput>> PostAsync<TInput,TOutput>(TInput input);
        Task<HttpRequestOutput> PutAsync();
        Task<HttpRequestOutput<TOutput>> PutAsync<TOutput>();
        Task<HttpRequestOutput> PutAsync<TInput>(TInput input);
        Task<HttpRequestOutput<TOutput>> PutAsync<TInput, TOutput>(TInput input);
        Task<HttpRequestOutput> DeleteAsync();
    }

}
