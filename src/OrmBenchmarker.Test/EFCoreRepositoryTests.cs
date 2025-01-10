namespace OrmBenchmarker.Test;

using Bogus;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OrmBenchmarker.Data.Frameworks;
using OrmBenchmarker.Data.Frameworks.EntityFramework;
using OrmBenchmarker.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class EFCoreRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly EFCoreRepository<Customer, int> _repository;

    private readonly int _customerRecords;
    private readonly int _ordersPerCustomer;

    private const string _connectionString = "your_connection_string_here";

    public EFCoreRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EFCoreRepository<Customer, int>(_context);

        _customerRecords = 1000;
        _ordersPerCustomer = 10;
    }

    public async Task SeedDatabaseAsync()
    {
        await DropTablesAsync();
        await CreateTablesAsync();

        var customers = DataGenerator.GenerateCustomers(_customerRecords);
        await _context.Customers.AddRangeAsync(customers);
        await _context.SaveChangesAsync();

        var orders = customers.SelectMany(c =>
            DataGenerator.GenerateOrders(_ordersPerCustomer, c.Id)).ToList();

        await _context.Orders.AddRangeAsync(orders);
        await _context.SaveChangesAsync();
    }

    private async Task DropTablesAsync()
    {
        var dropTablesQuery = @"
            IF OBJECT_ID('Orders', 'U') IS NOT NULL
                BEGIN
                    ALTER TABLE Orders DROP CONSTRAINT IF EXISTS FK_Orders_Customers;
                    DROP TABLE Orders;
                END;
            IF OBJECT_ID('Customers', 'U') IS NOT NULL DROP TABLE Customers;";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(dropTablesQuery, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateTablesAsync()
    {
        var createTablesQuery = @"
            CREATE TABLE Customers (
                Id INT IDENTITY PRIMARY KEY,
                Name NVARCHAR(100) NOT NULL,
                Email NVARCHAR(100) NOT NULL UNIQUE,
                CreatedAtUtc DATETIMEOFFSET NOT NULL
            );

            CREATE TABLE Orders (
                Id INT IDENTITY PRIMARY KEY,
                CustomerId INT NOT NULL,
                TotalAmount DECIMAL(18, 2) NOT NULL,
                OrderDate DATETIME NOT NULL,
                CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE CASCADE
            );";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(createTablesQuery, connection);
        await command.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
        await _repository.InsertAsync(customer);

        // Act
        var result = await _repository.GetByIdAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Name, result.Name);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedEntities()
    {
        // Arrange
        var customers = DataGenerator.GenerateCustomers(5);
        await _repository.InsertRangeAsync(customers);

        // Act
        var result = await _repository.GetPagedAsync(0, 2);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldReturnFilteredEntities()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Name = "John Doe", Email = "john.doe@example.com" },
            new Customer { Name = "Jane Doe", Email = "jane.doe@example.com" }
        };
        await _repository.InsertRangeAsync(customers);

        // Act
        var result = await _repository.GetFilteredAsync(c => c.Name.Contains("John"));

        // Assert
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].Name);
    }

    [Fact]
    public async Task InsertAsync_ShouldAddEntity()
    {
        // Arrange
        var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };

        // Act
        var result = await _repository.InsertAsync(customer);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(1, await _context.Customers.CountAsync());
    }

    [Fact]
    public async Task InsertRangeAsync_ShouldAddEntities()
    {
        // Arrange
        var customers = DataGenerator.GenerateCustomers(3);

        // Act
        await _repository.InsertRangeAsync(customers);

        // Assert
        Assert.Equal(3, await _context.Customers.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
        var insertedCustomer = await _repository.InsertAsync(customer);
        insertedCustomer.Name = "John Updated";

        // Act
        var updatedCustomer = await _repository.UpdateAsync(insertedCustomer);

        // Assert
        Assert.Equal("John Updated", updatedCustomer.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntityById()
    {
        // Arrange
        var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };
        var insertedCustomer = await _repository.InsertAsync(customer);

        // Act
        await _repository.DeleteAsync(insertedCustomer.Id);

        // Assert
        Assert.Equal(0, await _context.Customers.CountAsync());
    }
}

public static class DataGenerator
{
    public static List<Customer> GenerateCustomers(int count)
    {
        var faker = new Faker<Customer>()
            .RuleFor(c => c.Id, 0) // Database-generated
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.CreatedAtUtc, f => f.Date.PastOffset());

        return faker.Generate(count);
    }

    public static List<Order> GenerateOrders(int count, int customerId)
    {
        var faker = new Faker<Order>()
            .RuleFor(o => o.Id, 0) // Database-generated
            .RuleFor(o => o.CustomerId, customerId)
            .RuleFor(o => o.TotalAmount, f => f.Finance.Amount())
            .RuleFor(o => o.OrderDate, f => f.Date.Recent());

        return faker.Generate(count);
    }
}
