using System.ComponentModel.DataAnnotations;

namespace hrms_api.Dto
{
    public class SystemUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(100, ErrorMessage = "Username can be longer than 100 characters")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50,MinimumLength =6, ErrorMessage = "Password can be longer than 50 characters")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; }
    }
}
