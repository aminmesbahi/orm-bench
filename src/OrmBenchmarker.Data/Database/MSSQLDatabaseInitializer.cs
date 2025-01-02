using Dapper;
using Microsoft.Data.SqlClient;

namespace OrmBenchmarker.Data.Database;

public class MSSQLDatabaseInitializer : IDatabaseInitializer, IDisposable
{
    private readonly string _connectionString;
    private readonly SqlConnection _connection;

    public MSSQLDatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
        _connection = new SqlConnection(_connectionString);
    }

    /// <summary>
    /// Initializes the database schema.
    /// </summary>
    public async Task InitializeAsync()
    {
        await EnsureConnectionOpenAsync();
        await _connection.ExecuteAsync(@"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' AND xtype='U')
            CREATE TABLE Customers (
                Id INT PRIMARY KEY IDENTITY,
                Name NVARCHAR(100),
                Email NVARCHAR(100),
                CreatedAtUtc DATETIMEOFFSET
            );
            
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' AND xtype='U')
            CREATE TABLE Orders (
                Id INT PRIMARY KEY IDENTITY,
                CustomerId INT,
                TotalAmount DECIMAL(18, 2),
                OrderDate DATETIME,
                FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );
        ");
    }

    /// <summary>
    /// Seeds the database with fake data.
    /// </summary>
    /// <param name="count">Number of records to insert.</param>
    public async Task SeedDataAsync(int count)
    {
        await EnsureConnectionOpenAsync();

        var customers = DataGenerator.GenerateCustomers(count);
        var orders = DataGenerator.GenerateOrders(count);

        await _connection.ExecuteAsync(
            "INSERT INTO Customers (Name, Email, CreatedAtUtc) VALUES (@Name, @Email, @CreatedAtUtc);",
            customers
        );

        await _connection.ExecuteAsync(
            "INSERT INTO Orders (CustomerId, TotalAmount, OrderDate) VALUES (@CustomerId, @TotalAmount, @OrderDate);",
            orders
        );
    }

    /// <summary>
    /// Ensures the connection is open.
    /// </summary>
    private async Task EnsureConnectionOpenAsync()
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }
    }

    /// <summary>
    /// Disposes the connection when the object is no longer needed.
    /// </summary>
    public void Dispose()
    {
        if (_connection.State == System.Data.ConnectionState.Open)
        {
            _connection.Close();
        }
        _connection.Dispose();
    }
}
