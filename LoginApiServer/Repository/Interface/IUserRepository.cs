
using LoginApiServer.Model;

namespace LoginApiServer.Repository.Interface
{
    public interface IUserRepository
    {
        UserStatusCode CreateUser(User account);
        UserStatusCode DeleteUser(User user);
        UserStatusCode LoginUser(User request);
        UserStatusCode UpdateUser(User user);
    }
}
