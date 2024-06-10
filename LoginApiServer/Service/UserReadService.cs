using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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

        public UserResponse GetUserFromUserid(string id)
        {
            var user = _userRepository.GetUserFromUserid(id);
            
            if (user == null)
            {
                _logger.LogError("An error occurred while getting the User for UserId {UserId}.", id);
                return new UserResponse
                {
                    Status = UserStatusCode.Failure,
                    Message = "An error occurred while getting the User"
                };
            }
            else
            {
                var userResponse = new GetUserResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email
                };

                return new UserResponse
                {
                    Status = UserStatusCode.Success,
                    Message = "User retrieved successfully",
                    Content = Any.Pack(userResponse)
                };
            }
        }

    }
}
