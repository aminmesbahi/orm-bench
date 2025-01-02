namespace OrmBenchmarker.Data.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public virtual Customer Customer { get; set; }
}