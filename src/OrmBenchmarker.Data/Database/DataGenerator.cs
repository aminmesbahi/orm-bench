using Bogus;
using OrmBenchmarker.Data.Models;

namespace OrmBenchmarker.Data.Database;

public static class DataGenerator
{
    public static Customer GenerateCustomer() =>
        new Faker<Customer>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.CreatedAtUtc, f => f.Date.PastOffset())
            .Generate();

    public static IEnumerable<Customer> GenerateCustomers(int count) =>
        Enumerable.Range(1, count).Select(_ => GenerateCustomer());

    public static Order GenerateOrder(int customerId) =>
        new Faker<Order>()
            .RuleFor(o => o.CustomerId, _ => customerId)
            .RuleFor(o => o.TotalAmount, f => f.Finance.Amount())
            .RuleFor(o => o.OrderDate, f => f.Date.Past())
            .Generate();

    public static IEnumerable<Order> GenerateOrders(int count) =>
        Enumerable.Range(1, count).Select(i => GenerateOrder(i));
}
