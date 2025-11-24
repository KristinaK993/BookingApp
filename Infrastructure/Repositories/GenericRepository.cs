using System.Linq.Expressions;
using Domain.Common;
using Domain.Repositories;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    // "Table" for this type T – all ends up in this list
    private static readonly List<T> _items = new();
    private static int _nextId = 1;

    public Task<T?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<T>> ListAsync()
    {
        var result = _items.ToList();
        return Task.FromResult((IReadOnlyList<T>)result);
    }

    public Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate)
    {
        var compiled = predicate.Compile();
        var result = _items.Where(compiled).ToList();
        return Task.FromResult((IReadOnlyList<T>)result);
    }

    public Task<T> AddAsync(T entity)
    {
        entity.Id = _nextId++;
        _items.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity)
    {
        var existing = _items.FirstOrDefault(x => x.Id == entity.Id);
        if (existing is null)
        {
            return Task.CompletedTask;
        }

        var index = _items.IndexOf(existing);
        _items[index] = entity;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _items.RemoveAll(x => x.Id == entity.Id);
        return Task.CompletedTask;
    }
}
