using System.Linq.Expressions;

namespace blog_community_api.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T?> GetAsync(Guid id);
    Task<IEnumerable<T>> AllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    void Update(T entity);
    Task<bool> Delete(Guid id);
    Task<int> SaveChangesAsync();
}