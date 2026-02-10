using functstr.core;
using functstr.triggers.http.test.Helpers;
using functstr.triggers.http.test.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text;

namespace functstr.triggers.http.test
{
    public class HttpTriggerBindingResolverTests
    {
        private static ParameterInfo GetHttpTriggerParameter()
        {
            var method = typeof(FakeHttpTriggerFunction).GetMethod("Run", BindingFlags.Public | BindingFlags.Instance)!;
            return method.GetParameters()[0];
        }
            
        [Fact]
        public async Task CanResolve_ReturnsTrue_ForHttpTriggerParameter()
        {
            var resolver = new HttpTriggerBindingResolver(
                new NullTesterLogger(),
                Options.Create(new HttpTriggerOptions())
            );
            var parameter = GetHttpTriggerParameter();

            Assert.True(resolver.CanResolve(parameter));
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Resolve_Throws_WhenMethodMissing()
        {
            var logger = new CapturingTesterLogger();
            var options = new HttpTriggerOptions
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes("hello"))
            };
            var resolver = new HttpTriggerBindingResolver(
                logger,
                Options.Create(options)
            );

            var parameter = GetHttpTriggerParameter();

            Assert.Throws<InvalidOperationException>(() => resolver.Resolve(parameter));
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Resolve_Sets_Method_Headers_And_Body()
        {
            var logger = new CapturingTesterLogger();
            var options = new HttpTriggerOptions
            {
                Method = HttpMethod.Post,
                Body = new MemoryStream(Encoding.UTF8.GetBytes("payload")),
                Headers = new Dictionary<string, StringValues>
                {
                    ["x-header"] = new StringValues(new[]{"a","b"}),
                }
            };
            var resolver = new HttpTriggerBindingResolver(
                logger,
                Options.Create(options)
            );
            var parameter = GetHttpTriggerParameter();

            var request = (resolver.Resolve(parameter) as HttpRequest)!;

            Assert.Equal("POST", request.Method);
            Assert.True(request.Headers.ContainsKey("x-header"));
            Assert.Equal(new StringValues(new[]{"a","b"}), request.Headers["x-header"]);

            using var reader = new StreamReader(request.Body!, Encoding.UTF8, leaveOpen: true);
            var content = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
            Assert.Equal("payload", content);
        }

        [Fact]
        public async Task Resolve_Logs_Truncated_Body_When_Too_Large()
        {
            var logger = new CapturingTesterLogger();
            var largeString = new string('x', 2000);
            var options = new HttpTriggerOptions
            {
                Method = HttpMethod.Post,
                Body = new MemoryStream(Encoding.UTF8.GetBytes(largeString)),
            };
            var resolver = new HttpTriggerBindingResolver(
                logger,
                Options.Create(options)
            );
            var parameter = GetHttpTriggerParameter();

            var request = resolver.Resolve(parameter)!;

            var log = logger.ToString();
            Assert.Contains("(Content truncated to 1024 bytes.)", log);

            await Task.CompletedTask;
        }

        [Fact]
        public void test()
        {

            var mi = typeof(FakeHttpTriggerFunction).GetMethod("Run");

        }
    }
}
