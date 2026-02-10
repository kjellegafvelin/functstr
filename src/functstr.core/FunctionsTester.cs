using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;

namespace functstr.core
{
    public class FunctionsTester<TFunction> where TFunction : class
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ITesterLogger testerLogger;

        public static FunctionsTesterBuilder<TFunction> CreateBuilder() => new();

        internal FunctionsTester(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.testerLogger = serviceProvider.GetService<ITesterLogger>() ?? new NullTesterLogger();
        }


        private MethodInfo CreateFunctionMethod(string? functionName, TFunction instance)
        {
            List<MethodInfo> functionMethods;

            if (functionName == null || string.IsNullOrWhiteSpace(functionName))
            {
                functionMethods = instance
                    .GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => m.Name == functionName || m.GetCustomAttributes(inherit: false)
                                 .Any(a => a.GetType().Name == nameof(FunctionAttribute)))
                    .ToList();

                if (functionMethods.Count != 1)
                {
                    throw new InvalidOperationException($"Only one public method with FunctionAttribute is allowed when functionName is not specified. Found {functionMethods.Count} on type {typeof(TFunction).FullName}.");
                }
            }
            else
            {
                functionMethods = instance
                    .GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m =>
                    {
                        if (m.Name == functionName) return true;
                        var attr = m.GetCustomAttributes(inherit: false)
                                    .FirstOrDefault(a => a.GetType().Name == nameof(FunctionAttribute));
                        if (attr == null) return false;
                        var nameProp = attr.GetType().GetProperty(nameof(FunctionAttribute.Name));
                        var attrName = nameProp?.GetValue(attr) as string;
                        return string.Equals(attrName, functionName, StringComparison.Ordinal);
                    })
                    .ToList();

                if (functionMethods.Count != 1)
                {
                    throw new InvalidOperationException($"Only one public method with FunctionAttribute Name '{functionName}' is allowed. Found {functionMethods.Count} on type {typeof(TFunction).FullName}.");
                }
            }

            var targetMethod = functionMethods[0];
            return targetMethod;
        }

        private TFunction CreateInstance()
        {
            if (this.serviceProvider == null) throw new InvalidOperationException("ServiceProvider is not initialized.");

            var constructors = typeof(TFunction).GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"Type {typeof(TFunction).FullName} must have exactly one public constructor. Found {constructors.Length}.");
            }

            var ctor = constructors[0];
            var constructorParameters = ctor.GetParameters();

            object?[] args = Array.Empty<object?>();
            if (constructorParameters.Length > 0)
            {
                args = constructorParameters
                    .Select(p =>
                        this.serviceProvider.GetService(p.ParameterType)
                        ?? throw new InvalidOperationException($"Unable to resolve service for constructor parameter '{p.Name}' of type '{p.ParameterType.FullName}' in {typeof(TFunction).FullName}."))
                    .ToArray();
            }

            var instance = (TFunction)ctor.Invoke(args);
            return instance;
        }

        private object[] CreateFunctionParameters(MethodInfo targetMethod)
        {
            var resolver = this.serviceProvider.GetRequiredService<ITriggerBindingResolver>();

            var parameters = targetMethod.GetParameters();
            object[] newParameters = new object[parameters.Length];

            var (triggerValue, triggerIndex) = targetMethod.GetParameters().
                Where(x => x.GetCustomAttributes().Any(y => y.GetType().BaseType == typeof(TriggerBindingAttribute)))
                .Select((x, i) => (value: resolver.Resolve(x), index: i))
                .FirstOrDefault();

            if (triggerValue is not null)
            {
                newParameters[triggerIndex] = triggerValue!;
            }
            else
            {
                throw new InvalidOperationException("No trigger parameter could be resolved.");
            }

            return newParameters;
        }

        /// <summary>
        /// Searches for a function method named TFunction and is decorated with FunctionAttribute or if only one such method exists and runs it.
        /// </summary>
        /// <typeparam name="TResult">The expected result type.</typeparam>
        /// <returns>The <see cref="TestResult{TResult}"/> containing the result of the function execution.</returns>
        public async Task<TestResult<TResult>> RunAsync<TResult>()
        {
            return await this.RunAsync<TResult>(functionName: null).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs the specified function by name. If functionName is null or empty, searches for a function method decorated with FunctionAttribute or if only one such method exists and runs it.
        /// </summary>
        /// <typeparam name="TResult">The expected result type.</typeparam>
        /// <param name="functionName">Name of the function to run. Must be decorated with FunctionAttribute.</param>
        /// <returns>The <see cref="TestResult{TResult}"/> containing the result of the function execution.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<TestResult<TResult>> RunAsync<TResult>(string? functionName)
        {
            var instance = this.CreateInstance();

            MethodInfo targetMethod = this.CreateFunctionMethod(functionName, instance);

            object[] parameters = CreateFunctionParameters(targetMethod);

            this.testerLogger.Log("*-<( LOGGING )>-*\r\n");

            var stopwatch = Stopwatch.StartNew();
            object? invocationRaw = targetMethod.Invoke(instance, parameters);
            stopwatch.Stop();
            
            var returnType = targetMethod.ReturnType;

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                try
                {
                    var task = (Task<TResult>)invocationRaw!;
                    
                    await task.ConfigureAwait(false);

                    return CreateResult(task.Result, stopwatch);
                }
                catch (TargetInvocationException ex) when (ex.InnerException != null)
                {
                    // Unwrap TargetInvocationException to expose the actual exception thrown by the function
                    throw ex.InnerException;
                }
            }
            else
            {
                throw new NotSupportedException($"Return type '{returnType.FullName}' is not supported in this tester. Only Task and Task<T> are supported.");
            }
        }

        private static TestResult<TResult> CreateResult<TResult>(TResult? invocationResult, Stopwatch stopwatch)
        {
            // Minimal placeholder implementation
            return new TestResult<TResult>
            {
                Result = invocationResult,
                ElapsedTime = stopwatch.Elapsed
            };
        }
    }
}
