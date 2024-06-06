using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace LoginApiServer.Formatter
{
    public class ProtobufOutputFormatter : OutputFormatter
    {
        public ProtobufOutputFormatter()
        {
            SupportedMediaTypes.Add("application/x-protobuf");
        }

        protected override bool CanWriteType(Type type)
        {
            return typeof(IMessage).IsAssignableFrom(type);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            var message = (IMessage)context.Object;

            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);
                await response.Body.WriteAsync(stream.ToArray(), 0, (int)stream.Length);
            }
        }
    }
}
