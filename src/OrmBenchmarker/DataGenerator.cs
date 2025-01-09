using Bogus;
using OrmBenchmarker.Data.Models;

namespace OrmBenchmarker;

public static class DataGenerator
{
    public static List<Customer> GenerateCustomers(int count)
    {
        var customerFaker = new Faker<Customer>()
            .RuleFor(c => c.Id, f => 0) // Id will be set by the database
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.CreatedAtUtc, f => f.Date.PastOffset());

        return customerFaker.Generate(count);
    }

    public static List<Order> GenerateOrders(int count, int customerId)
    {
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.Id, f => 0) // Id will be set by the database
            .RuleFor(o => o.CustomerId, f => customerId)
            .RuleFor(o => o.TotalAmount, f => f.Finance.Amount())
            .RuleFor(o => o.OrderDate, f => f.Date.Past());

        return orderFaker.Generate(count);
    }
}