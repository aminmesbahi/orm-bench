namespace OrmBenchmarker;

using BenchmarkDotNet.Attributes;
using OrmBenchmarker.Data.Frameworks;
using OrmBenchmarker.Data.Frameworks.Dapper;
using OrmBenchmarker.Data.Frameworks.EntityFramework;
using OrmBenchmarker.Data.Models;
using System.Threading.Tasks;

public class Benchmarks
{
    private readonly EFCoreRepository<Customer, int> _efRepository;
    private readonly DapperRepository<Customer, int> _dapperRepository;

    public Benchmarks()
    {
        var connectionString = "YourConnectionStringHere";
        var context = new ApplicationDbContext(connectionString);
        _efRepository = new EFCoreRepository<Customer, int>(context);
        _dapperRepository = new DapperRepository<Customer, int>(connectionString);
    }

    [Benchmark]
    public async Task EFCore_GetByIdAsync()
    {
        await _efRepository.GetByIdAsync(1);
    }

    [Benchmark]
    public async Task Dapper_GetByIdAsync()
    {
        await _dapperRepository.GetByIdAsync(1);
    }
}
