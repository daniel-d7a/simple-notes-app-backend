
namespace todo_app.core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetOneByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        void Create(T data);

        void Update(T data);

        void Delete(T data);
    }
}
