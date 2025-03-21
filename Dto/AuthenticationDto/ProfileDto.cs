namespace hrms_api.Dto.AuthenticationDto;

public class ProfileDto
{
    public int EmployeeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public DateTime DOB { get; set; }
    public DateTime HiredDate { get; set; }
    public string PhoneNumber { get; set; }
    public string ImageUrl { get; set; }

    // SystemUser details
    public int SystemUserId { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
}