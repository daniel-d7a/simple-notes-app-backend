using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;
using todo_app.core.Repositories;

namespace todo_app.EF.Repositories;

using EntityFramework = Microsoft.EntityFrameworkCore.EF;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    protected ApplicationDbContext _context { get; set; }
    public GenericRepository(ApplicationDbContext context) {
        _context = context;
    }
    public async Task<T> GetOneByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

    //public async Task<IEnumerable<T>> GetAllAsync()
    //{
    //    return  _context.Set<T>();
    //}

    public void Create(T data)
    {
        _context.Set<T>().Add(data);
    }

    public void Update(T data)
    {
        _context.Set<T>().Update(data);
    }

    public void Delete(T data)
    {
        _context.Set<T>().Remove(data);
    }

    //public async Task<IEnumerable<T>> GetAllByUserIdAsync(string userId)
    //{
    //    return await _context.Set<T>()
    //        .Where(
    //        e => EntityFramework.Property<string>(e, "UserId")== userId).ToListAsync();
    //}

    public async Task<PaginatedResult<T>> GetAllByUserPaginatedAsync(string userId, BaseQueryParams queryParams)
    {
        var items = _context.Set<T>()
            .Where(e => EntityFramework.Property<string>(e, "UserId") == userId)
            .OrderBy(e => EntityFramework.Property<DateTime>(e, "CreatedAt"));

        return PaginatedResult<T>.ToPaginatedList(items, queryParams.Page, queryParams.PageSize);

    }
}
