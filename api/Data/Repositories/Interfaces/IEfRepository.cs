using System.Linq.Expressions;
using api.Models.Requests;
using api.Models.Responses;

namespace api.Data.Repositories.Interfaces
{
  public interface IEFRepository<T> where T : class
  {
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> include, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> include, CancellationToken cancellationToken = default);
    Task<PagedList<T>> FindManyPaginatedAsync(Expression<Func<T, bool>> predicate, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<T>> FindManyPaginatedAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> include, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>> include, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default);
  }
}