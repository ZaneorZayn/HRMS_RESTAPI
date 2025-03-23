namespace hrms_api.Dto;

public class SystemUserGetDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } // Optional: for displaying role details
}