using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using todo_app.core;
using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;
using todo_app.core.Repositories;

namespace todo_app.EF.Repositories;

using EntityFramework = Microsoft.EntityFrameworkCore.EF;

public class GenericRepository<T>(ApplicationDbContext _context) : IGenericRepository<T>
    where T : class
{
    public void Create(T data)
    {
        _context.Set<T>().Add(data);
    }

    public void CreateRange(IEnumerable<T> data)
    {
        _context.Set<T>().AddRange(data);
    }

    public void Delete(T data)
    {
        _context.Set<T>().Remove(data);
    }

    public void DeleteRange(IEnumerable<T> data)
    {
        _context.Set<T>().RemoveRange(data);
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null)
    {
        var items = _context.Set<T>().AsQueryable();

        if (filter is not null)
        {
            items = items.Where(filter);
        }
        return items;
    }

    public IEnumerable<T> GetAllByUser(
        string userId,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        string orderDirection = OrderDirections.Ascending
    )
    {
        var items = _context
            .Set<T>()
            .Where(e => EntityFramework.Property<string>(e, "UserId") == userId);

        if (filter is not null)
        {
            items = items.Where(filter);
        }

        if (orderBy is not null)
        {
            if (orderDirection == OrderDirections.Ascending)
                items = items.OrderBy(orderBy);
            else
                items = items.OrderByDescending(orderBy);
        }
        return items;
    }

    public PaginatedResult<T> GetAllByUserPaginated(
        string userId,
        BaseQueryParams queryParams,
        Expression<Func<T, bool>>? filter = null,
        params Expression<Func<T, object>>[] includes
    )
    {
        var items = _context
            .Set<T>()
            .Where(e => EntityFramework.Property<string>(e, "UserId") == userId);

        foreach (var include in includes)
        {
            items = items.Include(include);
        }

        if (filter is not null)
        {
            items = items.Where(filter);
        }

        items = items.OrderByDescending(e => EntityFramework.Property<DateTime>(e, "CreatedAt"));
        return PaginatedResult<T>.ToPaginatedList(items, queryParams.Page, queryParams.PageSize);
    }

    public T? GetOneById(
        int id,
        Expression<Func<T, object>>? includes = null,
        Expression<Func<object, object>>? thenIncludes = null
    )
    {
        var itemsIncluding = _context.Set<T>().AsQueryable();
        if (includes is not null)
        {
            if (thenIncludes is not null)
            {
                itemsIncluding = itemsIncluding.Include(includes).ThenInclude(thenIncludes);
            }
            else
            {
                itemsIncluding = itemsIncluding.Include(includes);
            }
        }

        return itemsIncluding.FirstOrDefault(e => EntityFramework.Property<int>(e, "Id") == id);
    }

    public void Update(T data)
    {
        _context.Set<T>().Update(data);
    }

    public void UpdateRange(IEnumerable<T> data)
    {
        _context.Set<T>().UpdateRange(data);
    }
}
