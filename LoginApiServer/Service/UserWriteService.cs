using Google.Protobuf.WellKnownTypes;
using LoginApiServer.Model;
using LoginApiServer.Repository.Interface;
using LoginApiServer.Service.Interface;
using System.Transactions;

namespace LoginApiServer.Service
{
    public class UserWriteService : IUserWriteService
    {
        private readonly ILogger<UserWriteService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICacheRepository _cacheRepository;

        public UserWriteService (ILogger<UserWriteService> logger, IUserRepository userRepository, ICacheRepository cacheRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _cacheRepository = cacheRepository;
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
                string sessionId = Guid.NewGuid().ToString().Replace("-", "");
                //var sessionIdToResponse = new StringValue
                //{
                //    Value = sessionId
                //};

                // Redis에 세션 ID 저장
                status = _cacheRepository.CreateSession(sessionId, user.Id);
                if (status != UserStatusCode.Success)
                {
                    _logger.LogError("An error occurred while login the User for UserId {UserId}.", request.UserId);
                    return new UserResponse
                    {
                        Status = status,
                        Message = "An error occurred while login the User"
                    };
                }

                // Chat Sever IP:Port, SessionID
                var loginSuccessResponse = new LoginResponse
                {
                    ServerIp = "127.0.0.1",
                    ServerPort = "4242",
                    SessionId = sessionId
                };

                return new UserResponse
                {
                    Status = status,
                    Message = (status == UserStatusCode.Success) ? "User Login successfully" : "User Login failed",
                    Content = Any.Pack(loginSuccessResponse)
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

        public UserResponse CreateUser(CreateUserRequest request)
        {
            var status = UserStatusCode.Success;
            var user = new User
            {
                UserId = request.UserId,
                Password = request.Password,
                Username = request.Username,
                Email = request.Email,
                IsAlive = true
            };


            try
            {
                status = _userRepository.CreateUser(user);

                return new UserResponse
                {
                    Status = status,
                    Message = (status == UserStatusCode.Success) ? "User created successfully" : "User creation failed",
                    Content = Any.Pack(request)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the User for UserId {UserId}.", request.UserId);
                return new UserResponse
                {
                    Status = status,
                    Message = $"An error occurred while creating the User: {ex.Message}"
                };
            }
        }

        public UserResponse UpdateUser(UpdateUserRequest request)
        {
            var status = UserStatusCode.Success;

            var user = new User
            {
                UserId = request.UserId,
                Password = request.Password
            };

            using (var scope = new TransactionScope())
            {
                try
                {
                    status = _userRepository.LoginUser(user);

                    if (status != UserStatusCode.Success)
                    {
                        _logger.LogError("An error occurred while updating the User for UserId {UserId}.", request.UserId);
                        return new UserResponse
                        {
                            Status = status,
                            Message = "An error occurred while updating the User"
                        };
                    }

                    // 업데이트할 필드만 업데이트
                    if (!string.IsNullOrEmpty(request.ToBePassword))
                    {
                        user.Password = request.ToBePassword;
                    }
                    if (!string.IsNullOrEmpty(request.ToBeUsername))
                    {
                        user.Username = request.ToBeUsername;
                    }
                    if (!string.IsNullOrEmpty(request.ToBeEmail))
                    {
                        user.Email = request.ToBeEmail;
                    }

                    // 사용자 정보를 업데이트
                    status = _userRepository.UpdateUser(user);
                    if (status != UserStatusCode.Success)
                    {
                        _logger.LogError("An error occurred while updating the User for UserId {UserId}.", request.UserId);
                        return new UserResponse
                        {
                            Status = status,
                            Message = "An error occurred while updating the User"
                        };
                    }

                    // 트랜잭션 범위 완료
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred while updating the User for UserId {UserId}.", request.UserId);
                    status = UserStatusCode.Failure;
                    return new UserResponse
                    {
                        Status = status,
                        Message = "An exception occurred while updating the User"
                    };
                }
            }

            return new UserResponse
            {
                Status = status,
                Message = (status == UserStatusCode.Success) ? "User updated successfully" : "User update failed"
            };
        }


        public UserResponse DeleteUser(DeleteUserRequest request)
        {
            var status = UserStatusCode.Success;

            var user = new User
            {
                UserId = request.UserId,
                Password = request.Password
            };

            using (var scope = new TransactionScope())
            {
                try
                {
                    status = _userRepository.LoginUser(user);

                    if (status != UserStatusCode.Success)
                    {
                        _logger.LogError("An error occurred while deleting the User for UserId {UserId}.", request.UserId);
                        return new UserResponse
                        {
                            Status = status,
                            Message = "An error occurred while deleting the User"
                        };
                    }

                    status = _userRepository.DeleteUser(user);
                    if (status != UserStatusCode.Success)
                    {
                        _logger.LogError("An error occurred while deleting the User for UserId {UserId}.", request.UserId);
                        return new UserResponse
                        {
                            Status = status,
                            Message = "An error occurred while deleting the User"
                        };
                    }

                    // TODO : Redis 세션 삭제

                    // 트랜잭션 범위 완료
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred while deleting the User for UserId {UserId}.", request.UserId);
                    status = UserStatusCode.Failure;
                    return new UserResponse
                    {
                        Status = status,
                        Message = "An exception occurred while deleting the User"
                    };
                }
            }

            return new UserResponse
            {
                Status = status,
                Message = (status == UserStatusCode.Success) ? "User deleted successfully" : "User deletion failed"
            };
        }

        
    }
}
