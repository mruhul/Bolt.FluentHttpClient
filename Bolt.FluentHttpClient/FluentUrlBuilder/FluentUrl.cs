using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bolt.FluentHttpClient.FluentUrlBuilder;

namespace Bolt.FluentHttpClient
{
    public class FluentUrl : IHaveFluentUrl, IHaveFluentPaths, IHaveFluentQs, IBuildFluentUrl
    {
        private readonly string _url;
        private List<Tuple<string, string>> _qs;
        private List<string> _paths;

        private FluentUrl(string url)
        {
            _url = url;
        }

        public static IHaveFluentUrl New(string url)
        {
            return new FluentUrl(url);
        }

        private const string StrSlash = "/";
        private const char ChrEq = '=';
        private const char ChrSlash = '/';
        private const char ChrAmp = '&';
        private const char ChrQs = '?';
        public string Build()
        {
            var hasQs = _qs != null && _qs.Count > 0;
            var hasPaths = _paths != null && _paths.Count > 0;

            if (!hasPaths && !hasQs) return _url;

            var urlParts = _url.Split(ChrQs);

            var hasPreExistingQs = urlParts.Length > 1;

            var url = urlParts[0];
            var sb = new StringBuilder(url);

            var hasEndSlash = url.EndsWith(StrSlash);
            if(hasPaths)
            {
                foreach(var path in _paths)
                {
                    sb.Append(hasEndSlash ? string.Empty : StrSlash).Append(path.TrimStart(ChrSlash));

                    hasEndSlash = path.EndsWith(StrSlash);
                }
            }

            if (!hasPreExistingQs && !hasQs) return sb.ToString();

            sb.Append(ChrQs);

            if (hasPreExistingQs)
            {
                sb.Append(urlParts[1]);

                if (hasQs)
                {
                    sb.Append(ChrAmp);
                }
                else
                {
                    return sb.ToString();
                }
            }

            foreach(var qs in _qs)
            {
                sb.Append(qs.Item1).Append(ChrEq).Append(qs.Item2).Append(ChrAmp);
            }

            return sb.ToString().TrimEnd(ChrAmp);
        }

        public IHaveFluentPaths Path(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return this;

            if (_paths == null) _paths = new List<string>();

            _paths.Add(path);

            return this;
        }

        public IHaveFluentPaths Path(params string[] paths)
        {
            if (paths == null) return this;

            if (_paths == null) _paths = new List<string>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path)) continue;

                _paths.Add(path);
            }

            return this;
        }

        public IHaveFluentQs QueryString(string name, string value, bool isValueEncoded = false)
        {
            if (value == null) return this;

            if (_qs == null) _qs = new List<Tuple<string, string>>();

            _qs.Add(new Tuple<string, string>(name, isValueEncoded ? value : Uri.EscapeDataString(value)));

            return this;
        }

        public IHaveFluentQs QueryString(string name, bool value)
        {
            if (_qs == null) _qs = new List<Tuple<string, string>>();

            _qs.Add(new Tuple<string, string>(name, value ? "true" : "false"));

            return this;
        }

        public IHaveFluentQs QueryString(string name, int value)
        {
            if (_qs == null) _qs = new List<Tuple<string, string>>();

            _qs.Add(new Tuple<string, string>(name, $"{value}"));

            return this;
        }

        public IHaveFluentQs QueryString(string name, double value)
        {
            if (_qs == null) _qs = new List<Tuple<string, string>>();

            _qs.Add(new Tuple<string, string>(name, $"{value}"));

            return this;
        }

        public IHaveFluentQs QueryString(string name, decimal value)
        {
            if (_qs == null) _qs = new List<Tuple<string, string>>();

            _qs.Add(new Tuple<string, string>(name, $"{value}"));

            return this;
        }

        public IHaveFluentQs QueryString<T>(T data, bool isValueEncoded = false)
        {
            if (data == null) return this;

            if (_qs == null) _qs = new List<Tuple<string, string>>();

            var properties = typeof(T).GetProperties().Where(x => x.CanRead);

            foreach(var p in properties)
            {
                var value = p.GetValue(data);

                if (value == null) continue;

                var strValue = $"{value}";

                _qs.Add(new Tuple<string, string>(p.Name, isValueEncoded ? strValue : Uri.EscapeDataString(strValue)));
            }

            return this;
        }

        public IHaveFluentQs QueryString(IDictionary<string, object> data, bool isValueEncoded = false)
        {
            if (data == null) return this;

            if (_qs == null) _qs = new List<Tuple<string, string>>();

            foreach(var keyValue in data)
            {
                if (keyValue.Value == null) continue;

                var strValue = $"{keyValue.Value}";

                _qs.Add(new Tuple<string, string>(keyValue.Key, isValueEncoded ? strValue : Uri.EscapeDataString(strValue)));
            }

            return this;
        }

        public IHaveFluentQs QueryString(Dictionary<string, object> data, bool isValueEncoded = false)
        {
            if (data == null) return this;

            if (_qs == null) _qs = new List<Tuple<string, string>>();

            foreach (var keyValue in data)
            {
                if (keyValue.Value == null) continue;

                var strValue = $"{keyValue.Value}";

                _qs.Add(new Tuple<string, string>(keyValue.Key, isValueEncoded ? strValue : Uri.EscapeDataString(strValue)));
            }

            return this;
        }
    }
}
