using todo_app.core.Models.Data;
using todo_app.core.Repositories;

namespace todo_app.EF.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenericRepository<Note> Notes { get; private set; }
        public IGenericRepository<Todo> Todos { get; private set; }
        public IGenericRepository<TodoEntry> TodoEntries { get; private set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Notes = new GenericRepository<Note>(_context);
            Todos = new GenericRepository<Todo>(_context);
            TodoEntries = new GenericRepository<TodoEntry>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
