using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.FluentHttpClient.FluentUrlBuilder
{
    public interface IHaveFluentUrl : ICollectFluentPaths, ICollectFluentQs
    { }
    public interface ICollectFluentUrl
    {
        IHaveFluentUrl Url(string url);
    }

    public interface IHaveFluentPaths : ICollectFluentPaths, ICollectFluentQs, IBuildFluentUrl
    { }
    public interface ICollectFluentPaths
    {
        IHaveFluentPaths Path(string path);
        IHaveFluentPaths Path(params string[] paths);
    }

    public interface IHaveFluentQs : ICollectFluentQs, IBuildFluentUrl
    { }
    public interface ICollectFluentQs
    {
        IHaveFluentQs QueryString(string name, bool value);
        IHaveFluentQs QueryString(string name, int value);
        IHaveFluentQs QueryString(string name, double value);
        IHaveFluentQs QueryString(string name, decimal value);
        IHaveFluentQs QueryString(string name, string value, bool isValueEncoded = false);
        IHaveFluentQs QueryString<T>(T data, bool isValueEncoded = false);
        IHaveFluentQs QueryString(IDictionary<string,object> data, bool isValueEncoded = false);
        IHaveFluentQs QueryString(Dictionary<string, object> data, bool isValueEncoded = false);
    }

    public interface IBuildFluentUrl
    {
        string Build();
    }
}
