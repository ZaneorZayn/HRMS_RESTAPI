namespace hrms_api.Model;

public class Position
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
