using System.Linq.Expressions;
using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;

namespace todo_app.core.Repositories
{
    public interface IGenericRepository<T>
        where T : class
    {
        void Create(T data);
        void CreateRange(IEnumerable<T> data);

        void Delete(T data);
        void DeleteRange(IEnumerable<T> data);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null);

        IEnumerable<T> GetAllByUser(
            string userId,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            string orderDirection = OrderDirections.Ascending
        );

        PaginatedResult<T> GetAllByUserPaginated(
            string userId,
            BaseQueryParams queryParams,
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes
        );

        T? GetOneById(
            int id,
            Expression<Func<T, object>>? includes = null,
            Expression<Func<object, object>>? thenIncludes = null
        );

        void Update(T data);
        void UpdateRange(IEnumerable<T> data);
    }
}
