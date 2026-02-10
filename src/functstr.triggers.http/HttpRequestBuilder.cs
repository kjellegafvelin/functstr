using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Text;

namespace functstr.triggers.http
{
    public class HttpRequestBuilder
    {
        private readonly Dictionary<string, StringValues> headers = [];
        private MemoryStream? body;
        private HttpMethod method = HttpMethod.Post;


        internal Dictionary<string, StringValues> Headers => headers;

        internal Stream? Body => body;

        internal HttpMethod Method => method;

        internal HttpRequestBuilder()
        {

        }

        public HttpRequestBuilder WithMethod(HttpMethod httpMethod)
        {
            ArgumentNullException.ThrowIfNull(httpMethod);
            this.method = httpMethod;
            return this;
        }

        public HttpRequestBuilder WithHeader(string name, string value)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(value);

            if (headers.TryGetValue(name, out StringValues existingValues))
            {
                headers[name] = StringValues.Concat(existingValues, value);
            }
            else
            {
                headers.Add(name, value);
            }

            return this;
        }

        public HttpRequestBuilder WithBody(string content)
        {
            ArgumentNullException.ThrowIfNull(content);
            var bytes = Encoding.UTF8.GetBytes(content);
            this.WithHeader("Content-Length", bytes.Length.ToString());
            body = new MemoryStream(bytes);

            return this;
        }

        public HttpRequestBuilder WithBodyFromResource(string resourceName)
        {
            using var reader = GetStream(resourceName);
            return this.WithBody(reader.ReadToEnd());
        }

        public static StreamReader GetStream(string resourceName, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            var coll = assembly.GetManifestResourceNames().Where(x => x.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));

            return coll.Count() switch
            {
                0 => throw new InvalidOperationException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'."),
                1 => new StreamReader(assembly.GetManifestResourceStream(coll.First())!),
                _ => throw new InvalidOperationException($"Multiple resources ending with '{resourceName}' found in assembly '{assembly.FullName}'. Specify a more specific resource name.")
            };
        }
    }
}
