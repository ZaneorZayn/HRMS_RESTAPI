namespace hrms_api.Dto.PermissionDto;

public class AssignPermissionDto
{
    public int RoleId { get; set; }
    public required List<int> PermissionIds { get; set; }
}