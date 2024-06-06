using Google.Protobuf.WellKnownTypes;
using LoginApiServer.Model;
using LoginApiServer.Repository.Interface;
using LoginApiServer.Service.Interface;

namespace LoginApiServer.Service
{
    public class UserReadService : IUserReadService
    {
        private readonly ILogger<UserReadService> _logger;
        private readonly IUserRepository _userRepository;

        public UserReadService(ILogger<UserReadService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public UserResponse LoginUser(LoginRequest request)
        {
            var status = UserStatusCode.Success;
            var user = new User
            {
                UserId = request.UserId,
                Password = request.Password
            };

            try
            {
                status = _userRepository.LoginUser(user);
                if (status != UserStatusCode.Success)
                {
                    return new UserResponse
                    {
                        Status = status,
                        Message = "User login failed"
                    };
                }

                // 세션ID 생성
                var uuid = new StringValue 
                { 
                    Value = Guid.NewGuid().ToString().Replace("-", "") 
                };

                // TODO : Redis 저장

                return new UserResponse
                {
                    Status = status,
                    Message = (status == UserStatusCode.Success) ? "User Login successfully" : "User Login failed",
                    Payload = Any.Pack(uuid)
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Login the User for UserId {UserId}.", request.UserId);
                return new UserResponse
                {
                    Status = status,
                    Message = $"An error occurred while Login the User: {ex.Message}"
                };
            }
        }
    }
}
