using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using hrms_api.Enum;

namespace hrms_api.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Gender Gender { get; set; }
        public DateTime DOB { get; set; }
        public DateTime HiredDate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }

        public int? SystemUserId { get; set; }
        [JsonIgnore]
        public SystemUser? SystemUser { get; set; }

        public int? DepartmentId { get; set; }   // nullable!
        public Department? Department { get; set; }

        public int? PositionId { get; set; }     // nullable if you want SET NULL
        public Position? Position { get; set; }
    }
}
