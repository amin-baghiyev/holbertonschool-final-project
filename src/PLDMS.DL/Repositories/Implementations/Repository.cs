using System.Linq.Expressions;
using code8hackathon.DL.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PLDMS.DL.Contexts;

namespace code8hackathon.DL.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class, new()
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public DbSet<T> Table => _context.Set<T>();

    public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, int page = 0,
        int count = 0, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool orderAsc = true,
        string orderBy = "Id", bool isTracking = false)
    {
        IQueryable<T> query = Table;

        if (!isTracking) query = query.AsNoTracking();

        if (includes is not null) query = includes(query);

        if (predicate is not null) query = query.Where(predicate);

        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, orderBy);
        var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

        query = orderAsc ? query.OrderBy(lambda) : query.OrderByDescending(lambda);

        if (count > 0) query = query.Skip(page * count).Take(count);

        return await query.ToListAsync();
    }

    public async Task<T?> GetOneAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool isTracking = false)
    {
        IQueryable<T> query = Table;

        if (!isTracking) query = query.AsNoTracking();

        if (includes is not null) query = includes(query);

        return await query.SingleOrDefaultAsync(predicate);
    }

    public async Task CreateAsync(T entity)
    {
        await Table.AddAsync(entity);
    }

    public void Update(T entity)
    {
        Table.Update(entity);
    }

    public void Delete(T entity)
    {
        Table.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}