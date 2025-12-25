namespace hrms_api.Dto.AuthenticationDto
{
    public class LoginResponeDto
    {

        public string? Token { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public DateTime Expiration { get; set; }
        
        public string RefreshToken { get; set; }
    }
}
