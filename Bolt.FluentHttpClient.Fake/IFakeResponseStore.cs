using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bolt.FluentHttpClient.Fake
{
    public interface IFakeResponseStore
    {
        IEnumerable<MockResponse> Get();
    }


    public class FileBasedFakeResponseStore : IFakeResponseStore
    {
        private static readonly string DirName = Path.Combine(Environment.CurrentDirectory,"HttpFakes");
        private readonly ILogger<FileBasedFakeResponseStore> _logger;

        public FileBasedFakeResponseStore(ILogger<FileBasedFakeResponseStore> logger)
        {
            _logger = logger;
        }

        public IEnumerable<MockResponse> Get()
        {
            if (Directory.Exists(DirName))
            {
                return ReadFromDirectory(DirName);
            }

            _logger.LogError($"Directory {DirName} does not exists. Please ensure you put all your fake http responses inside this directory.");

            return Enumerable.Empty<MockResponse>();
        }

        private IEnumerable<MockResponse> ReadFromDirectory(string dirPath)
        {
            var files = Directory.GetFiles(dirPath);

            if (files != null)
            {
                foreach(var file in files)
                {
                    var result = ReadFromFile(file);

                    foreach(var item in result)
                    {
                        yield return item;
                    }
                }
            }

            var directories = Directory.GetDirectories(dirPath);

            foreach(var dir in directories)
            {
                var result = ReadFromDirectory(dir);

                foreach(var item in result)
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<MockResponse> ReadFromFile(string filePath)
        {
            return JsonConvert.DeserializeObject<IEnumerable<MockResponse>>(File.ReadAllText(filePath));
        }
    }
}
