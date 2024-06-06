using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;

namespace LoginApiServer.Utils
{
    public static class ProtobufResultHelper
    {
        public static UserResponse CreateErrorResult(UserStatusCode statusCode, string message)
        {
            return new UserResponse
            {
                Status = statusCode,
                Message = message
            };
        }
    }
}
