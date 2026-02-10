using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace functstr.core
{
    public class FunctionsTesterBuilder<TFunction> where TFunction : class
    {
        private ITesterLogger testerLogger = new NullTesterLogger();

        internal FunctionsTesterBuilder()
        {
            
        }

        public IServiceCollection Services { get; } = new ServiceCollection();

        public FunctionsTester<TFunction> Build()
        {
            if (!Services.Any(s => s.ServiceType == typeof(ILogger<TFunction>)))
            {
                this.Services.AddSingleton<ILogger<TFunction>, NullLogger<TFunction>>();
            }

            if (!Services.Any(s => s.ServiceType == typeof(ITesterLogger)))
            {
                this.Services.AddSingleton<ITesterLogger>(testerLogger);
            }

            return new FunctionsTester<TFunction>(Services.BuildServiceProvider());
        }
    }
}