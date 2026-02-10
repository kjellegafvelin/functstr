using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace functstr.triggers.http.test.TestDoubles
{
    public class FakeHttpTriggerFunction
    {
        [Function("FakeHttpTriggerFunction")]
        public async Task<ContentResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            return new ContentResult();
        }
    }
}
