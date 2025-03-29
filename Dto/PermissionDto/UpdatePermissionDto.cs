using System.ComponentModel.DataAnnotations;

namespace hrms_api.Dto.PermissionDto;

public class UpdatePermissionDto
{
    [Required(ErrorMessage = "Permission name is required")]
    [MaxLength(50, ErrorMessage = "Permission name cannot be longer than 50 characters")]
    public required string PermissionName { get; set; }
}