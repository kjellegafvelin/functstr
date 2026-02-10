using funcstr.testing.xUnit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace functstr.core
{
    public static class FunctionsTesterBuilderExtensions
    {
        public static FunctionsTesterBuilder<TFunction> UseTestOutputHelper<TFunction>(this FunctionsTesterBuilder<TFunction> builder, ITestOutputHelper testOutputHelper) where TFunction : class
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);
            var logger = new TestOutputHelperLogger<TFunction>(testOutputHelper);
            builder.Services.AddSingleton<ITesterLogger>(logger);
            builder.Services.AddSingleton<ILogger<TFunction>>(logger);
            return builder;
        }

    }
}
