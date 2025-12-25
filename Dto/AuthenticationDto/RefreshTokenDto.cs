namespace hrms_api.Dto.AuthenticationDto;

public class RefreshTokenDto
{
    public string TokenType { get; set; } = "Bearer";
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
}