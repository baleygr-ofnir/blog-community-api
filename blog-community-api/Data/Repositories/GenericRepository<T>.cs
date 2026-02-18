using System.Linq.Expressions;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace blog_community_api.Data.Repositories;

public abstract class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly BlogContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(BlogContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }
    
    public virtual async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }
    
    public virtual async Task<T?> GetAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> AllAsync()
    {
        return await DbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet
            .Where(predicate)
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual async Task<bool> Delete(Guid id)
    {
        var entity = await GetAsync(id);
        if (entity == null) return false;
        DbSet.Remove(entity);
        return true;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await Context.SaveChangesAsync();
    }
}