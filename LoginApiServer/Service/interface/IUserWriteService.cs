

namespace LoginApiServer.Service.Interface
{
    public interface IUserWriteService
    {
        UserResponse LoginUser(LoginRequest request);
        UserResponse CreateUser(CreateUserRequest request);
        UserResponse DeleteUser(DeleteUserRequest request);
        UserResponse UpdateUser(UpdateUserRequest request);
    }
}
