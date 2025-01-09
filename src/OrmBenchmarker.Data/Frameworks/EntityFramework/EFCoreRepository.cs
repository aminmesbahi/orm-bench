namespace OrmBenchmarker.Data.Frameworks;

using Microsoft.EntityFrameworkCore;
using OrmBenchmarker.Data.Frameworks.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class EFCoreRepository<T, TKey> : IRepository<T, TKey> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public EFCoreRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<T>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Skip(pageIndex * pageSize)
                           .Take(pageSize)
                           .ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetFilteredAsync(Func<T, bool> filter, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _dbSet.AsNoTracking().Where(filter).ToList(), cancellationToken);
    }

    public async Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

