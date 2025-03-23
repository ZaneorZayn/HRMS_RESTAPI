using System.ComponentModel.DataAnnotations;

namespace hrms_api.Dto;

public class SystemUserEditDto
{
    [Required]
    public  string Username { get; set; }

    [Required]
    public int RoleId { get; set; }
}