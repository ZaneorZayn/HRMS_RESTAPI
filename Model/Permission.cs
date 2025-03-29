using System.Text.Json.Serialization;

namespace hrms_api.Model;

public class Permission
{
    public int PermissionId { get; set; }
    
    public string? PermissionName { get; set; }
    [JsonIgnore]
    public ICollection<RolePermission>? RolePermissions { get; set; }
}