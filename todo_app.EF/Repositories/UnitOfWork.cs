using todo_app.core;
using todo_app.core.Models.Data;
using todo_app.core.Repositories;

namespace todo_app.EF.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IWithLabelRepository<Note, LabelNote> Notes { get; private set; }
        public IWithLabelRepository<Todo, LabelTodo> Todos { get; private set; }
        public IGenericRepository<TodoEntry> TodoEntries { get; private set; }
        public IGenericRepository<Label> Labels { get; private set; }
        public ILabelJoinRepository<LabelNote> LabelNote { get; private set; }
        public ILabelJoinRepository<LabelTodo> LabelTodo { get; private set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Notes = new WithLabelsRepository<Note, LabelNote>(_context);
            Todos = new WithLabelsRepository<Todo, LabelTodo>(_context);
            TodoEntries = new GenericRepository<TodoEntry>(_context);
            Labels = new GenericRepository<Label>(_context);
            LabelNote = new LabelJoinRepository<LabelNote>(_context);
            LabelTodo = new LabelJoinRepository<LabelTodo>(_context);
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
