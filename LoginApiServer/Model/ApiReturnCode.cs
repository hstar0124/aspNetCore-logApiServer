namespace LoginApiServer.Model
{
    public enum ApiReturnCode
    {
        Success,                    // 성공
        BadRequest,                 // 잘못된 요청

        AccountAlreadyExists,       // 이미 존재하는 계정
        NoAccount,                  // 없는 계정
        InvalidCredentials,         // 유효하지 않은 자격 증명
        AccountDisabled,            // 비활성화된 계정
        AccountLockedOut,           // 잠긴 계정

        ExpiredSession,             // 만료된 세션

        ServerError                 // 서버 내부 오류
    }
}