using System.Linq.Expressions;
using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;
using todo_app.core.Repositories;

namespace todo_app.core;

public interface IWithLabelRepository<T, U> : IGenericRepository<T>
    where T : class
{
    PaginatedResult<T> GetAllByUserWithLabelsPaginated(
        string userId,
        BaseQueryParams queryParams,
        Expression<Func<T, object>>[]? includes = null,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object>>? orderBy = null,
        string orderDirection = OrderDirections.Ascending
    );
    T? GetWithLabels(int id, params Expression<Func<T, object>>[] includes);
}
