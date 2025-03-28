using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hrms_api.Model;

public class RolePermission
{
    
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    
    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
    
    [ForeignKey("PermissionId")]
    public Permission? Permission { get; set; }
    
    
    
}