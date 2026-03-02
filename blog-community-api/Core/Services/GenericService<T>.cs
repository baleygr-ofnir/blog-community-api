using System.Linq.Expressions;
using AutoMapper;
using blog_community_api.Core.Interfaces;
using blog_community_api.Data.Repositories;
using blog_community_api.Security;

namespace blog_community_api.Core.Services;

public abstract class GenericService<T> : IService<T> where T : class
{
    protected IRepository<T> Repository;
    protected readonly IMapper Mapper;

    public GenericService(IRepository<T> repository, IMapper mapper)
    {
        Repository = repository;
        Mapper = mapper;
    }
    
    public virtual async Task<T> AddAsync(T entity)
    {
        var added = await Repository.AddAsync(entity);
        await Repository.SaveChangesAsync();

        return added;
    }

    public virtual async Task<T?> GetAsync(Guid id) => await Repository.GetAsync(id);

    public virtual async Task<IEnumerable<T>> AllAsync() => await Repository.AllAsync();

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await Repository.FindAsync(predicate);
    
    public virtual async Task<T?> Update(Guid id, T entity)
    {
        var existing = await Repository.GetAsync(id);
        if (existing is null) return null;
        
        Repository.Update(entity);
        await Repository.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await Repository.DeleteAsync(id);
        if (deleted) await Repository.SaveChangesAsync();
        
        return deleted;
    }
}