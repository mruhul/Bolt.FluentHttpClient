using Bolt.FluentHttpClient.Abstracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.FluentHttpClient.Abstracts.Fluent
{
    public interface IHaveUrl : ICollectHeaders, ICollectRetryCount, ICollectTimeout, ICollectOnFailure, ISendMessage
    {

    }

    public interface ICollectUrl
    {
        IHaveUrl Url(string url);
    }


    public interface ICollectHeaders
    {
        IHaveHeaders Header(params HttpHeader[] headers);
        IHaveHeaders Header(HttpHeader header);
        IHaveHeaders Header(string name, string value);
        IHaveHeaders Header(IEnumerable<HttpHeader> headers);
    }

    public interface IHaveHeaders : ICollectRetryCount, ICollectTimeout, ICollectOnFailure, ISendMessage
    {

    }


    public interface ICollectRetryCount
    {
        IHaveRetryCount Retry(int count);
    }
    public interface IHaveRetryCount : ICollectTimeout, ICollectOnFailure, ISendMessage
    {

    }

    public interface ICollectTimeout 
    {
        IHaveTimeout Timeout(TimeSpan timeout);
        IHaveTimeout TimeoutInMilliseconds(int ms);
        IHaveTimeout TimeoutInSeconds(int seconds);
        IHaveTimeout TimeoutInMinutes(int minutes);
    }
    public interface IHaveTimeout : ICollectTimeout, ICollectOnFailure, ISendMessage
    {

    }

    public interface IHaveOnFailure : ISendMessage
    {
        IHaveOnFailure OnFailure<T>(HttpStatusCode statusCode, Action<T> handler);
        IHaveOnFailure OnBadRequest<T>(Action<T> handler);
    }

    public interface ICollectOnFailure
    {
        IHaveOnFailure OnFailure(Action<IHttpOnFailureInput> handler);
        IHaveOnFailure OnFailure<T>(HttpStatusCode statusCode, Action<T> handler);
        IHaveOnFailure OnBadRequest<T>(Action<T> handler);
    }

    public interface ISendMessage
    {
        Task<TypedHttpMessageOutput<TOutput>> GetAsync<TOutput>();
        Task<TypedHttpMessageOutput<TOutput>> PostAsync<TOutput>();
        Task<TypedHttpMessageOutput<TOutput>> PutAsync<TOutput>();


        Task<TypedHttpMessageOutput<TOutput>> PostAsync<TInput, TOutput>(TInput input);
        Task<TypedHttpMessageOutput<TOutput>> PutAsync<TInput, TOutput>(TInput input);

        Task<TypedHttpMessageOutput> DeleteAsync();
        Task<TypedHttpMessageOutput> PostAsync();
        Task<TypedHttpMessageOutput> PutAsync();
    }
}
