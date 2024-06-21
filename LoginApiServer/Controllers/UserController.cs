using Google.Protobuf.WellKnownTypes;
using LoginApiServer.Model;
using LoginApiServer.Service.Interface;
using LoginApiServer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace LoginApiServer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        // CQRS 패턴 적용
        // 명령을 처리하는 책임과 조회를 처리하는 책임을 분리
        private readonly IUserWriteService _accountWriteService;
        private readonly IUserReadService _accountReadService;

        public UserController(ILogger<UserController> logger, IUserWriteService accountWriteService, IUserReadService accountReadService)
        {
            _logger = logger;
            _accountWriteService = accountWriteService;
            _accountReadService = accountReadService;
        }

        [HttpGet]
        public UserResponse GetUser([FromQuery]string userId)
        {
           
            try
            {
                var response = _accountReadService.GetUserFromUserid(userId);
                return response;
            }
            catch (Exception ex)
            {
                return ProtobufResultHelper.CreateErrorResult(UserStatusCode.ServerError, $"An error occurred while getting the User: {ex.Message}");
            }
        }

        // 유저 생성
        [HttpPost]
        public UserResponse Create([FromBody] CreateUserRequest request)
        {
            try
            {
                var response = _accountWriteService.CreateUser(request);

                return response;
            }
            catch (Exception ex)
            {
                return ProtobufResultHelper.CreateErrorResult(UserStatusCode.ServerError, $"An error occurred while creating the User: {ex.Message}");
            }
        }

        // 유저 로그인
        [HttpPost]
        public UserResponse Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = _accountWriteService.LoginUser(request);

                return response;
            }
            catch (Exception ex)
            {
                return ProtobufResultHelper.CreateErrorResult(UserStatusCode.ServerError, $"An error occurred while loging the User: {ex.Message}");
            }
        }

        // 유저 변경
        [HttpPost]
        public UserResponse Update([FromBody] UpdateUserRequest request)
        {
            try
            {
                var response = _accountWriteService.UpdateUser(request);

                return response;

            }
            catch (Exception ex)
            {

                return ProtobufResultHelper.CreateErrorResult(UserStatusCode.ServerError, $"An error occurred while updating the User: {ex.Message}");
            }
        }

        // 유저 삭제
        [HttpPost]
        public UserResponse Delete([FromBody] DeleteUserRequest request)
        {
            try
            {
                var response = _accountWriteService.DeleteUser(request);

                return response;
            }
            catch (Exception ex)
            {

                return ProtobufResultHelper.CreateErrorResult(UserStatusCode.ServerError, $"An error occurred while delete the User: {ex.Message}");
            }

        }
    }
}
