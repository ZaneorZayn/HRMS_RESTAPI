namespace hrms_api.Dto.AuthenticationDto;

public class EditProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DOB { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Image { get; set; } // For uploading profile images
}