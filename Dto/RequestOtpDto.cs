using System.ComponentModel.DataAnnotations;

namespace hrms_api.Dto
{
    public class RequestOtpDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(100, ErrorMessage = "Email must be less than 100 characters.")]
        public string Email { get; set; }
    }
}
