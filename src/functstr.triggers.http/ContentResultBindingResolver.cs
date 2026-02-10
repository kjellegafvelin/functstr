using funcstr.triggers.http;
using functstr.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using System.Reflection;

namespace functstr.triggers.http
{
    internal class ContentResultBindingResolver : IOutputBindingResolver<ContentResult>
    {
        private readonly ITesterLogger testerLogger;

        public ContentResultBindingResolver(ITesterLogger testerLogger)
        {
            this.testerLogger = testerLogger;
        }

        public bool CanResolve(ParameterInfo parameterInfo)
        {
            return parameterInfo.ParameterType == typeof(ContentResult);
        }

        public TestResult<ContentResult> Resolve(ParameterInfo parameterInfo)
        {
            throw new NotImplementedException();
        }

        //public async Task<HttpTriggerTestResult> RunAsync(string? functionName)
        //{
        //    T instance = base.CreateInstance();

        //    MethodInfo targetMethod = base.CreateFunctionMethod(functionName, instance);

        //    object[] parameters = CreateFunctionParameters(targetMethod);

        //    base.TesterLogger.Log("*-<( LOGGING )>-*\r\n");

        //    var stopwatch = Stopwatch.StartNew();
        //    object? invocationRaw = targetMethod.Invoke(instance, parameters);
        //    stopwatch.Stop();

        //    var returnType = targetMethod.ReturnType;

        //    object? invocationResult;

        //    if (typeof(Task).IsAssignableFrom(returnType))
        //    {
        //        try
        //        {
        //            var task = (Task)invocationRaw!;

        //            await task.ConfigureAwait(false);

        //            if (returnType.IsGenericType)
        //            {
        //                // Task<T>: capture Result
        //                invocationResult = returnType.GetProperty("Result")?.GetValue(task);
        //            }
        //            else
        //            {
        //                // Non-generic Task
        //                invocationResult = null;
        //            }

        //            return CreateResult(invocationResult, stopwatch);
        //        }
        //        catch (TargetInvocationException ex) when (ex.InnerException != null)
        //        {
        //            // Unwrap TargetInvocationException to expose the actual exception thrown by the function
        //            throw ex.InnerException;
        //        }
        //    }
        //    else
        //    {
        //        throw new NotSupportedException($"Return type '{returnType.FullName}' is not supported in this tester. Only Task and Task<T> are supported.");
        //    }

        //}

        private HttpTriggerTestResult CreateResult(object? invocationResult, Stopwatch stopwatch)
        {
            this.testerLogger.Log("\r\n*-<( RESPONSE )>-*\r\n");
            this.testerLogger.Log($"Elapsed: {stopwatch.Elapsed.Milliseconds} ms");

            if (invocationResult is not null)
            {
                this.testerLogger.Log($"Result Type: {invocationResult.GetType().Name}");
            }

            if (invocationResult is IStatusCodeActionResult statusCodeActionResult)
            {
                this.testerLogger.Log($"Status Code: {statusCodeActionResult.StatusCode}");
            }

            if (invocationResult is ContentResult contentResult)
            {
                this.testerLogger.Log($"Content Type: {contentResult.ContentType}");
                if (contentResult.Content != null)
                {
                    var maxLength = 1024;
                    this.testerLogger.Log($"Content Length: {contentResult.Content.Length}");
                    var displayLength = Math.Min(maxLength, contentResult.Content.Length);
                    this.testerLogger.Log($"Content:\r\n\r\n{contentResult.Content[..displayLength]}");

                    if (contentResult.Content.Length > maxLength)
                    {
                        this.testerLogger.Log($"\r\n(Content truncated to {maxLength} characters.)");
                    }
                }
            }

            return new HttpTriggerTestResult
            {
                Result = invocationResult
            };
        }



        //public override HttpTriggerTestResult Run(string? functionName)
        //{
        //    T instance = base.CreateInstance();

        //    MethodInfo targetMethod = base.CreateFunctionMethod(functionName, instance);

        //    object[] parameters = CreateFunctionParameters(targetMethod);

        //    object? invocationRaw = targetMethod.Invoke(instance, parameters);

        //    var returnType = targetMethod.ReturnType;

        //    object? invocationResult;

        //    // Synchronous method:
        //    if (returnType == typeof(void))
        //    {
        //        invocationResult = null;
        //    }
        //    else
        //    {
        //        invocationResult = invocationRaw;
        //    }

        //    return new HttpTriggerTestResult
        //    {
        //        Result = invocationResult
        //    };
        //}

    }
}
