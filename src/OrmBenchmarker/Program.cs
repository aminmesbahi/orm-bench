using Microsoft.Extensions.Configuration;
using OrmBenchmarker;
IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

var seeder = new DatabaseSeeder(configuration);
await seeder.SeedDatabaseAsync();

var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmarks>();