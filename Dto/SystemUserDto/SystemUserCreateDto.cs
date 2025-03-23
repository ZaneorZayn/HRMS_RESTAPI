namespace hrms_api.Dto;

public class SystemUserCreateDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
   
}