
using Google.Protobuf;
using LoginApiServer.Model;

namespace LoginApiServer.Service.Interface
{
    public interface IUserReadService
    {
        UserResponse GetUserFromUserid(string id);
    }
}
