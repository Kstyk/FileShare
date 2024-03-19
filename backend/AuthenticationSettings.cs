namespace backend
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public int JwtAccessExpiresMinutes { get; set; }
        public int JwtRefreshExpiresDays { get; set; }
        public string JwtIssuer { get; set; }
    }
}
