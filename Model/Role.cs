﻿namespace hrms_api.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
