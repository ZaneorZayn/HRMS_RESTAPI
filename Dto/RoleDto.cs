using System.ComponentModel.DataAnnotations;

namespace hrms_api.Dto
{
    public class RoleDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name can be longer than 100 characters")]
        public string? Name { get; set; }

        
    }
}
