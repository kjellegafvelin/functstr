using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace funcstr.triggers.http
{
    public class HttpTriggerTestResult
    {
        public int? StatusCode 
        { 
            get
            {
                if (Result is IStatusCodeActionResult statusCodeResult)
                    return statusCodeResult.StatusCode;
                return null;
            }
        }

        public object? Result { get; internal set; }
    }
}
