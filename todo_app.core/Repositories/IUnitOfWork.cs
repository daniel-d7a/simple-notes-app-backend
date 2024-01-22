using todo_app.core.Models.Data;

namespace todo_app.core.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IWithLabelRepository<Note, LabelNote> Notes { get; }
        IWithLabelRepository<Todo, LabelTodo> Todos { get; }
        IGenericRepository<TodoEntry> TodoEntries { get; }
        IGenericRepository<Label> Labels { get; }
        ILabelJoinRepository<LabelNote> LabelNote { get; }
        ILabelJoinRepository<LabelTodo> LabelTodo { get; }
        int SaveChanges();
    }
}
