
using todo_app.core.Helpers.Pagination;
using todo_app.core.Helpers.QueryParams;

namespace todo_app.core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetOneByIdAsync(int id);

        //Task<IEnumerable<T>> GetAllAsync();

        //Task <IEnumerable<T>> GetAllByUserIdAsync(string userId);

        Task<PaginatedResult<T>> GetAllByUserPaginatedAsync(string userId, BaseQueryParams queryParams);

        void Create(T data);

        void Update(T data);

        void Delete(T data);
    }
}
