namespace backend.Models.Responses
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public LoginResponseDto(string access, string refresh)
        {
            AccessToken = access;
            RefreshToken = refresh;
        }
    }
}
