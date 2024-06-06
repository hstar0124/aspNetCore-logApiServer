
using Google.Protobuf;

namespace LoginApiServer.Service.Interface
{
    public interface IUserReadService
    {
        UserResponse LoginUser(LoginRequest request);
    }
}
