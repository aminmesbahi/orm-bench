namespace OrmBenchmarker.Data.Frameworks.Dapper;

using global::Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class DapperRepository<T, TKey> : IRepository<T, TKey> where T : class
{
    private readonly string _connectionString;

    public DapperRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"SELECT * FROM {typeof(T).Name}s WHERE Id = @Id";
        return await connection.QueryFirstOrDefaultAsync(query, new { Id = id });
    }

    public async Task<List<T>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"SELECT * FROM {typeof(T).Name}s ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        return (await connection.QueryAsync<T>(query, new { Offset = pageIndex * pageSize, PageSize = pageSize })).ToList();
    }

    public async Task<List<T>> GetFilteredAsync(Func<T, bool> filter, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"SELECT * FROM {typeof(T).Name}s";
        var result = (await connection.QueryAsync<T>(query)).ToList();
        return result.Where(filter).ToList();
    }

    public async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"INSERT INTO {typeof(T).Name}s OUTPUT INSERTED.* VALUES (@Entity)";
        return await connection.QuerySingleAsync<T>(query, new { Entity = entity });
    }

    public async Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"INSERT INTO {typeof(T).Name}s VALUES (@Entity)";
        await connection.ExecuteAsync(query, entities.Select(e => new { Entity = e }));
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"UPDATE {typeof(T).Name}s SET @Entity OUTPUT INSERTED.* WHERE Id = @Id";
        return await connection.QuerySingleAsync<T>(query, new { Entity = entity, Id = ((dynamic)entity).Id });
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"DELETE FROM {typeof(T).Name}s WHERE Id = @Id";
        await connection.ExecuteAsync(query, new { Id = id });
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = $"DELETE FROM {typeof(T).Name}s WHERE Id = @Id";
        await connection.ExecuteAsync(query, new { Id = ((dynamic)entity).Id });
    }
}
