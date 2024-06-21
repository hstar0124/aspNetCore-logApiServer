using LoginApiServer.Model;
using LoginApiServer.Repository.Interface;
using StackExchange.Redis;

namespace LoginApiServer.Repository
{
    public class CacheRepositoryFromRedis : ICacheRepository
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly int _sessionDbIndex = 0;
        private readonly IDatabase _sessionDb;

        public CacheRepositoryFromRedis()
        {
            _redis = ConnectionMultiplexer.Connect("127.0.0.1");
            _sessionDb = _redis.GetDatabase(_sessionDbIndex);
        }

        public UserStatusCode CreateSession(string sessionId, long accountId)
        {
            try
            {
                var accountKey = $"Account:{accountId}";
                var sessionKey = $"Session:{sessionId}";

                // 먼저 AccountID로 조회
                RedisValue existingSessionId = _sessionDb.StringGet(accountKey);
                if (existingSessionId.HasValue)
                {
                    var existingSessionKey = $"Session:{existingSessionId}";

                    var tran = _sessionDb.CreateTransaction();

                    // 기존 세션 삭제 및 새로운 세션 설정
                    tran.KeyDeleteAsync(existingSessionKey);
                    tran.StringSetAsync(sessionKey, accountId, new TimeSpan(0, 3, 0));
                    tran.StringSetAsync(accountKey, sessionId, new TimeSpan(0, 3, 0));

                    bool committed = tran.Execute();
                    return committed ? UserStatusCode.Success : UserStatusCode.Failure;
                }
                else
                {
                    // 새로운 세션을 생성
                    var tran = _sessionDb.CreateTransaction();

                    tran.AddCondition(Condition.KeyNotExists(sessionKey));
                    tran.AddCondition(Condition.KeyNotExists(accountKey));

                    tran.StringSetAsync(sessionKey, accountId, new TimeSpan(0, 3, 0));
                    tran.StringSetAsync(accountKey, sessionId, new TimeSpan(0, 3, 0));

                    bool committed = tran.Execute();
                    return committed ? UserStatusCode.Success : UserStatusCode.Failure;
                }
            }
            catch (Exception)
            {
                return UserStatusCode.ServerError;
            }
        }

        public UserStatusCode KeepAliveSessionFromAccountId(long accountId)
        {
            try
            {
                string accountKey = $"Account:{accountId}";
                RedisValue redisValue = _sessionDb.StringGet(accountKey);
                if (!redisValue.HasValue)
                {
                    return UserStatusCode.Failure;
                }

                var sessionId = redisValue.ToString();
                var sessionKey = $"Session:{sessionId}";

                var tran = _sessionDb.CreateTransaction();

                tran.KeyExpireAsync(accountKey, new TimeSpan(0, 3, 0));
                tran.KeyExpireAsync(sessionKey, new TimeSpan(0, 3, 0));

                bool committed = tran.Execute();
                return committed ? UserStatusCode.Success : UserStatusCode.Failure;
            }
            catch (Exception)
            {
                return UserStatusCode.ServerError;
            }
        }

        public UserStatusCode DeleteSessionFromAccountId(long accountId)
        {
            try
            {
                string accountKey = $"Account:{accountId}";
                RedisValue redisValue = _sessionDb.StringGet(accountKey);
                if (!redisValue.HasValue)
                {
                    return UserStatusCode.Failure;
                }

                string sessionId = redisValue.ToString();
                string sessionKey = $"Session:{sessionId}";

                var tran = _sessionDb.CreateTransaction();

                tran.KeyDeleteAsync(sessionKey);
                tran.KeyDeleteAsync(accountKey);

                bool committed = tran.Execute();
                return committed ? UserStatusCode.Success : UserStatusCode.Failure;
            }
            catch (Exception)
            {
                return UserStatusCode.ServerError;
            }
        }

    }
}
