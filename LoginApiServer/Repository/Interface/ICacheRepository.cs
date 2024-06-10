using LoginApiServer.Model;

namespace LoginApiServer.Repository.Interface
{
    public interface ICacheRepository
    {
        UserStatusCode CreateSession(string sessionId, long id);
    }
}
