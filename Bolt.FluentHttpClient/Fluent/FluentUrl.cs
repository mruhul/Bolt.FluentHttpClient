using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bolt.FluentHttpClient.Fluent
{
    public class FluentUrl : ICollectFluentUrlPath, IHaveFluentUrlPath, IHaveQueryString
    {
        private readonly List<string> _paths = new List<string>();
        private readonly Dictionary<string, string> _qs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private const char QsStart = '?';
        private const char QsSep = '&';
        private const char QsValueSep = '=';
        private const char PathSep = '/';

        private FluentUrl(){}

        public static ICollectFluentUrlPath New()
        {
            return new FluentUrl();
        }

        public static ICollectFluentUrlPath FromUrl(string url)
        {
            return new FluentUrl().Path(url);
        }

        public IHaveFluentUrlPath Path(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return this;

            var qsIndex = path.IndexOf(QsStart);

            if(qsIndex != -1)
            {
                var qs = path.Substring(qsIndex + 1);
                AdQuerystring(qs);

                path = path.Substring(0, qsIndex);
            }

            _paths.Add(path);

            return this;
        }

        IHaveFluentUrlPath ICollectFluentUrlPath.Path(params string[] paths)
        {
            if (paths == null) return this;

            foreach(var path in paths)
            {
                Path(path);
            }

            return this;
        }

        IHaveFluentUrlPath ICollectFluentUrlPath.Path(IEnumerable<string> paths)
        {
            if (paths == null) return this;

            foreach (var path in paths)
            {
                Path(path);
            }

            return this;
        }

        private void AdQuerystring(string qs)
        {
            if (string.IsNullOrWhiteSpace(qs)) return;

            var data = qs.Split(QsSep);

            foreach(var item in data)
            {
                var keyValue = item.Split(QsValueSep);
                _qs[keyValue[0]] = keyValue.Length == 1 ? string.Empty : keyValue[1];
            }
        }

        public string Build()
        {
            var sb = new StringBuilder();

            var totalPaths = _paths.Count;
            for(var i = 0; i < totalPaths; i++)
            {
                var path = _paths[i];
                
                if(i == 0)
                {
                    if(i < totalPaths - 1)
                    {
                        path = path.TrimEnd(PathSep);
                        sb.Append(path).Append(PathSep);
                    }
                    else
                    {
                        sb.Append(path);
                    }
                }
                else
                {
                    if (i < totalPaths - 1)
                    {
                        path = path.Trim(PathSep);
                        sb.Append(path).Append(PathSep);
                    }
                    else
                    {
                        path = path.TrimStart(PathSep);
                        sb.Append(path);
                    }
                }
            }

            var totalQs = _qs.Count;
            if (totalQs > 0) sb.Append(QsStart);

            foreach(var q in _qs)
            {
                sb.AppendFormat("{0}={1}{2}", q.Key, q.Value, QsSep);
            }

            return sb.ToString().TrimEnd(QsSep);
        }

        public IHaveQueryString QueryString(string name, string value, bool encoded = false)
        {
            if (value == null) return this;

            _qs[name] = encoded ? value : Uri.EscapeDataString(value);

            return this;
        }

        public IHaveQueryString QueryStrings(Dictionary<string, string> data)
        {
            if (data == null) return this;
            foreach(var item in data)
            {
                QueryString(item.Key, item.Value);
            }
            return this;
        }

        public IHaveQueryString QueryStrings(IDictionary<string, string> data)
        {
            if (data == null) return this;
            foreach (var item in data)
            {
                QueryString(item.Key, item.Value);
            }
            return this;
        }

        public IHaveQueryString QueryStrings<T>(T data)
        {
            if (data == null) return this;
            var properties = data.GetType().GetProperties().Where(p => p.CanRead);

            foreach(var p in properties)
            {
                QueryString(p.Name, p.GetValue(data)?.ToString());
            }

            return this;
        }
    }

    public interface IHaveFluentUrlPath : ICollectFluentUrlPath, ICollectionQueryString
    {
        string Build();
    }
    public interface ICollectFluentUrlPath
    {
        IHaveFluentUrlPath Path(string path);
        IHaveFluentUrlPath Path(params string[] paths);
        IHaveFluentUrlPath Path(IEnumerable<string> paths);
    }

    public interface IHaveQueryString: ICollectionQueryString
    {
        string Build();
    }

    public interface ICollectionQueryString
    {
        IHaveQueryString QueryString(string name, string value, bool encoded = false);
        IHaveQueryString QueryStrings(Dictionary<string,string> data);
        IHaveQueryString QueryStrings(IDictionary<string, string> data);
        IHaveQueryString QueryStrings<T>(T data);
    }
}
