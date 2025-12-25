using System.ComponentModel.DataAnnotations.Schema;

namespace hrms_api.Model;

public class Department
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int? ManagerId { get; set; }      // nullable for SET NULL
    [ForeignKey("ManagerId")]
    public Employee? Manager { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}