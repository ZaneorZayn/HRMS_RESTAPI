using System.ComponentModel.DataAnnotations;
using hrms_api.Enum;

namespace hrms_api.Dto;

public class CreateEmployeeDto
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage ="Name can be longer than 100 characters ")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }
    
    [Required(ErrorMessage = "DOB is required")]
    [DataType(DataType.Date)]
    public DateTime DOB { get; set; }

    [Required(ErrorMessage = "Hired Date is required")]
    [DataType(DataType.Date)]
    public DateTime HiredDate { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    [MaxLength(255, ErrorMessage = "Address can be longer than 255 characters ")]
    public string? Address { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    [MaxLength(255, ErrorMessage = "Address can be longer than 255 characters ")]
    public string? PhoneNumber { get; set; }
    
    public string? ImageUrl { get; set; }
    
    

}