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
        private readonly ICacheRepository _cacheRepository;

        public UserReadService(ILogger<UserReadService> logger, IUserRepository userRepository, ICacheRepository cacheRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
        }

        
    }
}
