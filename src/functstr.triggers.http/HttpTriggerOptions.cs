using Microsoft.Extensions.Primitives;

namespace functstr.triggers.http
{
    public class HttpTriggerOptions
    {
        public HttpMethod? Method { get; set; }

        public Stream? Body { get; set; }

        public string? BodyContent { get; set; }

        public Dictionary<string, StringValues> Headers { get; set; } = [];
    }
}