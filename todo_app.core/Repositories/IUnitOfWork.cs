using todo_app.core.Models.Data;

namespace todo_app.core.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Note> Notes { get; }
        IGenericRepository<Todo> Todos { get; }
        IGenericRepository<TodoEntry> TodoEntries { get; }
        int SaveChanges();
    }
}
