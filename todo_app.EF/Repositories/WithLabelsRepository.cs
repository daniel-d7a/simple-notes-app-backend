using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using todo_app.core;
using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;
using todo_app.EF.Repositories;
using EFCore = Microsoft.EntityFrameworkCore.EF;

namespace todo_app.EF;

public class WithLabelsRepository<TData, TJoin>
    : GenericRepository<TData>,
        IWithLabelRepository<TData, TJoin>
    where TData : class
{
    private readonly ApplicationDbContext _context;

    public WithLabelsRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public PaginatedResult<TData> GetAllByUserWithLabelsPaginated(
        string userId,
        BaseQueryParams queryParams,
        Expression<Func<TData, object>>[]? includes = null,
        Expression<Func<TData, bool>>? filter = null,
        Expression<Func<TData, object>>? orderBy = null,
        string orderDirection = OrderDirections.Ascending
    )
    {
        var items = _context.Set<TData>().AsQueryable();

        if (includes is not null)
        {
            foreach (var include in includes)
            {
                items = items.Include(include);
            }
        }

        items = items
            .Include(e => EFCore.Property<TJoin>(e, "LabelData"))
            .ThenInclude(e => EFCore.Property<Label>(e!, "Label"))
            .Where(e => EFCore.Property<string>(e, "UserId") == userId);

        if (filter is not null)
        {
            items = items.Where(filter);
        }

        if (orderBy is not null)
        {
            if (orderDirection == OrderDirections.Ascending)
            {
                items = items.OrderBy(orderBy);
            }
            else
            {
                items = items.OrderByDescending(orderBy);
            }
        }
        else
        {
            items = items.OrderByDescending(e => EFCore.Property<DateTime>(e, "CreatedAt"));
        }

        return PaginatedResult<TData>.ToPaginatedList(
            items,
            queryParams.Page,
            queryParams.PageSize
        );
    }

    public TData? GetWithLabels(int id, params Expression<Func<TData, object>>[] includes)
    {
        var items = _context.Set<TData>().AsQueryable();

        foreach (var include in includes)
        {
            items = items.Include(include);
        }

        return items
            .Include(e => EFCore.Property<TJoin>(e, "LabelData"))
            .ThenInclude(e => EFCore.Property<Label>(e!, "Label"))
            .FirstOrDefault(e => EFCore.Property<int>(e, "Id") == id);
    }
}
