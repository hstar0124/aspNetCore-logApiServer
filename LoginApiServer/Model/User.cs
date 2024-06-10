namespace LoginApiServer.Model
{
    public class User
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public bool? IsAlive { get; set; }

    }
}
