namespace OrmBenchmarker.Data.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public List<Order> Orders { get; set; } = default!;
}
