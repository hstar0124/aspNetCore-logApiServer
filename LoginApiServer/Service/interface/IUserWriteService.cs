

namespace LoginApiServer.Service.Interface
{
    public interface IUserWriteService
    {
        UserResponse CreateUser(CreateUserRequest request);
        UserResponse DeleteUser(DeleteUserRequest request);
        UserResponse UpdateUser(UpdateUserRequest request);
    }
}
