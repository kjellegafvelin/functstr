using functstr.core;
using functstr.triggers.http.test.TestDoubles;
using Microsoft.AspNetCore.Mvc;

namespace functstr.triggers.http.test
{
    public class FunctionTesterTests
    {
        [Fact]
        public async Task SampleTest()
        {
            var builder = FunctionsTester<FakeHttpTriggerFunction>.CreateBuilder();
            
            builder.UseTestOutputHelper(TestContext.Current.TestOutputHelper!);
            
            builder.ConfigureHttpRequest(options =>
            {
                options.Method = HttpMethod.Get;
                options.Headers.Add("X-Custom-Header", "HeaderValue");
                options.BodyContent = "Test body content";
            });

            var tester = builder.Build();
            var result = await tester.RunAsync<ContentResult>();

            Assert.NotNull(result.Result);

        }

    }
}
