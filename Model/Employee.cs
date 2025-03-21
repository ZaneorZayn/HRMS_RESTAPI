using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hrms_api.Model
{
    public class Employee
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public DateTime DOB { get; set; }

        public DateTime HiredDate { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public string? ImageUrl { get; set; }

        public string? Address { get; set; }

        public int? SystemUserId { get; set; } // Optional link to SystemUser

        [ForeignKey("SystemUserId")]
        [JsonIgnore]
        public SystemUser? SystemUser { get; set; }
    }
}
