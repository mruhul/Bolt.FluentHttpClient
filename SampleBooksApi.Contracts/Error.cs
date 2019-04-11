using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBooksApi.Contracts
{
    public class ErrorResponse
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string,string[]> Errors { get; set; }
    }
}
