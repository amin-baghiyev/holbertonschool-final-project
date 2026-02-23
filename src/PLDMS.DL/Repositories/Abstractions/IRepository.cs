using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace PLDMS.DL.Repositories.Abstractions;

public interface IRepository<T> where T : class, new()
{
    DbSet<T> Table { get; }

    Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, int page = 0, int count = 0,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool orderAsc = true,
        string orderBy = "Id", bool isTracking = false);

    Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool isTracking = false);

    Task CreateAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync();
}