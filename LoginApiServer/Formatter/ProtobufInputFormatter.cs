using Google.Protobuf;
using LoginApiServer.Utils;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace LoginApiServer.Formatter
{
    public class ProtobufInputFormatter : InputFormatter
    {
        public ProtobufInputFormatter()
        {
            SupportedMediaTypes.Add("application/x-protobuf");
        }

        protected override bool CanReadType(Type type)
        {
            return typeof(IMessage).IsAssignableFrom(type);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var httpRequest = context.HttpContext.Request;

            if (httpRequest.ContentLength == null || httpRequest.ContentLength == 0)
            {
                return await InputFormatterResult.NoValueAsync();
            }

            var message = (IMessage)Activator.CreateInstance(context.ModelType);

            try
            {
                using (var stream = new MemoryStream())
                {
                    await httpRequest.Body.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    message.MergeFrom(stream.ToArray());
                }
                return await InputFormatterResult.SuccessAsync(message);
            }
            catch (Exception ex)
            {
                // Handle the exception here
                var failResponse = ProtobufResultHelper.CreateErrorResult(UserStatusCode.ServerError, ex.Message);
                var failureResponseBytes = failResponse.ToByteArray();
                return await InputFormatterResult.FailureAsync();
            }
        }
    }
}
