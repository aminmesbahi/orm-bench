namespace OrmBenchmarker.Data.Database;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
    Task SeedDataAsync(int count);
}