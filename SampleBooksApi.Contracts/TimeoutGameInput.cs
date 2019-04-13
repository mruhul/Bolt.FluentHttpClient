using System;
using System.Collections.Generic;
using System.Text;

namespace SampleBooksApi.Contracts
{
    public class TimeoutGameInput
    {
        public string Name { get; set; }
        public int DelayInMs { get; set; }
        public int DelayCount { get; set; }
    }

    public class TimeoutGameResponse
    {
        public int TotalHit { get; set; }
    }
}
