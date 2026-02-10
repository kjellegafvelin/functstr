using functstr.core;
using Microsoft.Extensions.Logging;
using Xunit;

namespace funcstr.testing.xUnit
{
    internal class TestOutputHelperLogger<T> : ITesterLogger, ILogger<T> where T : class
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TestOutputHelperLogger(ITestOutputHelper testOutputHelper)
        {
            ArgumentNullException.ThrowIfNull(testOutputHelper);
            this.testOutputHelper = testOutputHelper;
        }

        public void Log(string message)
        {
            this.testOutputHelper.WriteLine(message);
        }

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = $"{DateTime.Now} {logLevel.ToString().ToLower()}: {formatter(state, exception)}";
            this.testOutputHelper.WriteLine(message);
        }
    }
}
