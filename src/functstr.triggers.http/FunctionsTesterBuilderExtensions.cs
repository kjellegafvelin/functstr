using functstr.triggers.http;
using Microsoft.Extensions.DependencyInjection;

namespace functstr.core
{
    public static class FunctionsTesterBuilderExtensions
    {
        public static FunctionsTesterBuilder<TFunction> ConfigureHttpRequest<TFunction>(this FunctionsTesterBuilder<TFunction> builder, Action<HttpTriggerOptions> options) where TFunction : class
        {
            ArgumentNullException.ThrowIfNull(options);
            //var triggerOptions = new HttpTriggerOptions();
            //options(triggerOptions);
            
            builder.Services.Configure<HttpTriggerOptions>(options);

            builder.Services.AddSingleton<ITriggerBindingResolver, HttpTriggerBindingResolver>();

            return builder;
        }
    }
}
