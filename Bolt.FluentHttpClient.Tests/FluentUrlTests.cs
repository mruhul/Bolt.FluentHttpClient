using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Bolt.FluentHttpClient.Tests
{
    public class FluentUrlTests
    {
        [Fact]
        public void Should_Build_Currect_Url()
        {
            string result;
            result = FluentUrl.New("http://www.google.com").Path().Build();
            result.ShouldBe("http://www.google.com");

            result = FluentUrl.New("http://www.google.com?id=1").Path().Build();
            result.ShouldBe("http://www.google.com?id=1");


            result = FluentUrl.New("http://www.google.com?id=1").Path("test").Build();
            result.ShouldBe("http://www.google.com/test?id=1");


            result = FluentUrl.New("http://www.google.com/?id=1").Path("test").Build();
            result.ShouldBe("http://www.google.com/test?id=1");


            result = FluentUrl.New("http://www.google.com/?id=1").Path("test","/hello/").QueryString("id",2).Build();
            result.ShouldBe("http://www.google.com/test/hello/?id=1&id=2");


            result = FluentUrl.New("http://www.google.com/?id=1").QueryString("id", 2).Build();
            result.ShouldBe("http://www.google.com/?id=1&id=2");

            result = FluentUrl.New("http://www.google.com").Path("test", "hello")
                .QueryString("id", "e r")
                .QueryString("isTrue", true)
                .Build();
            result.ShouldBe("http://www.google.com/test/hello?id=e%20r&isTrue=true");
        }
    }
}
