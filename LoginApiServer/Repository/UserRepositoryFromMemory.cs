using LoginApiServer.Model;
using LoginApiServer.Repository.Interface;
using LoginApiServer.Utils;
using System.Collections.Concurrent;

namespace LoginApiServer.Repository
{
    public class UserRepositoryFromMemory : IUserRepository
    {
        private readonly ILogger<UserRepositoryFromMemory> _logger;
        private readonly ConcurrentDictionary<string, User> _users;

        // 나중에 디비에서 오토인크리먼트로 빼자.
        private long _id = 0;



        public UserRepositoryFromMemory(ILogger<UserRepositoryFromMemory> logger)
        {
            _logger = logger;
            _users = new ConcurrentDictionary<string, User>();
        }

        public User? GetUserFromUserid(string userId)
        {
            if (!_users.ContainsKey(userId))
            {
                _logger.LogError("User retrieval failed for UserId {UserId}. UserId does not exist.", userId);
                return null;
            }

            return _users[userId];
        }

        public UserStatusCode CreateUser(User userInfo)
        {
            // 아이디가 이미 존재하는 경우
            if (_users.ContainsKey(userInfo.UserId))
            {
                _logger.LogError("User creation failed for UserId {UserId}. UserId already exists.", userInfo.UserId);
                return UserStatusCode.UserIdAlreadyExists;
            }

            // 아토믹 처리 필요
            userInfo.Id = ++_id;

            // 비밀번호 해시
            userInfo.Password = PasswordHelper.HashPassword(userInfo.Password);

            // 사용자 정보 저장
            if (!_users.TryAdd(userInfo.UserId, userInfo))
            {
                _logger.LogError("User creation failed for UserId {UserId}.", userInfo.UserId);
                return UserStatusCode.Failure;
            }

            _logger.LogInformation("User with UserId {UserId} created successfully.", userInfo.UserId);
            return UserStatusCode.Success;
        }

        public UserStatusCode DeleteUser(User userInfo)
        {
            _users[userInfo.UserId].IsAlive = false;
            return UserStatusCode.Success;
        }



        public UserStatusCode LoginUser(User userInfo)
        {
            if (!_users.ContainsKey(userInfo.UserId) || _users[userInfo.UserId].IsAlive == false)
            {
                _logger.LogError("User not exists :: {UserId}", userInfo.UserId);
                return UserStatusCode.UserNotExists;
            }

            var storedUser = _users[userInfo.UserId];

            if (!PasswordHelper.VerifyPassword(storedUser.Password, userInfo.Password))
            {
                _logger.LogError("Incorrect password for user :: {UserId}", userInfo.UserId);
                return UserStatusCode.DifferentPassword;
            }

            _logger.LogInformation("User login successfully :: {UserId}", userInfo.UserId);
            return UserStatusCode.Success;
        }

        public UserStatusCode UpdateUser(User userInfo)
        {
            if (!string.IsNullOrEmpty(userInfo.Username))
            {
                _users[userInfo.UserId].Username = userInfo.Username;
            }
            if (!string.IsNullOrEmpty(userInfo.Email))
            {
                _users[userInfo.UserId].Email = userInfo.Email;
            }
            if (!string.IsNullOrEmpty(userInfo.Password))
            {
                _users[userInfo.UserId].Password = PasswordHelper.HashPassword(userInfo.Password);
            }

            _logger.LogInformation("User with UserId {UserId} updated successfully.", userInfo.UserId);
            return UserStatusCode.Success;
        }
    }
}
