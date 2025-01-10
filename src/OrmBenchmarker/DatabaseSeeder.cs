namespace OrmBenchmarker;

using global::OrmBenchmarker.Data.Frameworks.EntityFramework;
using global::OrmBenchmarker.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly int _customerRecords;
    private readonly int _orderPerCustomer;
    private readonly string _connectionString;

    public DatabaseSeeder(IConfiguration configuration)
    {
        _connectionString = configuration.GetSection("Databases:MSSQL").Value;
        _customerRecords = int.Parse(configuration.GetSection("Benchmarks:CustomerRecords").Value);
        _orderPerCustomer = int.Parse(configuration.GetSection("Benchmarks:OrderPerCustomer").Value);
        _context = new ApplicationDbContext(_connectionString);
    }

    public async Task SeedDatabaseAsync()
    {
        await DropTablesAsync();
        await CreateTablesAsync();

        var customers = DataGenerator.GenerateCustomers(_customerRecords);
        await _context.Customers.AddRangeAsync(customers);
        await _context.SaveChangesAsync();
        var customerIds = customers.Select(c => c.Id).ToList();
        var orders = new List<Order>();

        foreach (var customerId in customerIds)
        {
            orders.AddRange(DataGenerator.GenerateOrders(_orderPerCustomer, customerId));
        }


        await _context.Orders.AddRangeAsync(orders);
        await _context.SaveChangesAsync();

    }

    private async Task DropTablesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var dropTablesQuery = @"
            IF OBJECT_ID('Orders', 'U') IS NOT NULL DROP TABLE Orders;
            IF OBJECT_ID('Customers', 'U') IS NOT NULL DROP TABLE Customers;";
        using var command = new SqlCommand(dropTablesQuery, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task CreateTablesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var createTablesQuery = @"
            CREATE TABLE Customers (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100),
                Email NVARCHAR(100),
                CreatedAtUtc DATETIMEOFFSET
            );
            
            CREATE TABLE Orders (
                Id INT PRIMARY KEY IDENTITY,
                CustomerId INT,
                TotalAmount DECIMAL(18, 2),
                OrderDate DATETIME,
                FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );";
        using var command = new SqlCommand(createTablesQuery, connection);
        await command.ExecuteNonQueryAsync();
    }
}
