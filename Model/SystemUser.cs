using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hrms_api.Model
{
    public class SystemUser
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public int RoleId { get; set; }
        
        public ICollection<Attendance>? Attendances { get; set; }

        [JsonIgnore]
        public Role? Role { get; set; }
    }
}
