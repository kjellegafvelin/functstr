using funcstr.triggers.http;
using functstr.core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text;

namespace functstr.triggers.http
{
    public class HttpTriggerBindingResolver : ITriggerBindingResolver
    {
        private readonly ITesterLogger testerLogger;
        private readonly HttpTriggerOptions options;

        public HttpTriggerBindingResolver(ITesterLogger testerLogger, IOptions<HttpTriggerOptions> options)
        {
            this.testerLogger = testerLogger;
            this.options = options.Value;
        }

        public bool CanResolve(ParameterInfo parameterInfo)
        {
            return parameterInfo.GetCustomAttributes(inherit: false)
                .Any(a => a.GetType().Name == nameof(HttpTriggerAttribute));
        }

        public object? Resolve(ParameterInfo parameterInfo)
        {
            this.testerLogger.Log("*-<( REQUEST )>-*\r\n");

            var request = new DefaultHttpContext().Request;

            if (this.options.Method is null) 
            {
                throw new InvalidOperationException("Request method is missing");
            }

            request.Method = this.options.Method.ToString().ToUpper();
            this.testerLogger.Log($"Method: {request.Method}");

            this.testerLogger.Log($"Headers:");

            foreach (var header in this.options.Headers)
            {
                this.testerLogger.Log($"\t{header.Key} = {header.Value}");
                request.Headers.Append(header.Key, header.Value);
            }


            if (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put || request.Method == HttpMethods.Patch)
            {
                if (options.Body is not null)
                {
                    request.Body = new HttpRequestStream(this.options.Body ?? Stream.Null);
                }
                else if (options.BodyContent is not null)
                {
                    request.Body = new HttpRequestStream(new MemoryStream(Encoding.UTF8.GetBytes(this.options.BodyContent)));
                }
                else
                {
                    throw new InvalidOperationException("Request body is missing");
                }
            }

            if (request.Body is HttpRequestStream stream && stream.InternalStream.CanSeek)
            {
                var maxLength = 1024;
                Span<byte> buffer = stackalloc byte[(int)Math.Min(maxLength, stream.InternalStream.Length)];
                stream.InternalStream.Read(buffer);
                stream.InternalStream.Seek(0, SeekOrigin.Begin);

                this.testerLogger.Log("\r\nContent:\r\n");
                this.testerLogger.Log(Encoding.UTF8.GetString(buffer));

                if (stream.InternalStream.Length > maxLength)
                {
                    this.testerLogger.Log($"\r\n(Content truncated to {maxLength} bytes.)\r\n");
                }
            }

            return request;
        }
    }
}
