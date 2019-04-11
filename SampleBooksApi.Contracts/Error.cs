using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBooksApi.Contracts
{
    public class Error
    {
        public string Code { get; set; }
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }

    public class ErrorContainer
    {
        public IEnumerable<Error> Errors { get; set; }
    }
}
