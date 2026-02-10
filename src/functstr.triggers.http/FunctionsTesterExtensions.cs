using functstr.core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace functstr.triggers.http
{
    public static class FunctionsTesterExtensions
    {
        public static HttpRequest CreateHttpRequest<TFunction>(this FunctionsTester<TFunction> tester) where TFunction : class 
        {
            
            return new DefaultHttpContext().Request;
        }

    }
}
