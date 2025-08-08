using api.Data.Context;
using api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Implementations
{
  public class EfRepository<T> : IEFRepository<T> where T : class
  {
    private readonly TaskContext _context;
    public EfRepository(TaskContext context)
    {
      _context = context;
    }

    public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
      var keyValues = new object[] { id };
      return await _context.Set<T>().FindAsync(keyValues, cancellationToken);
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
      var keyValues = new object[] { id };
      return await _context.Set<T>().FindAsync(keyValues, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
      return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
      await _context.Set<T>().AddAsync(entity, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
      _context.Set<T>().Update(entity);
      await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
      var entity = await GetByIdAsync(id, cancellationToken);
      if (entity != null)
      {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }
}