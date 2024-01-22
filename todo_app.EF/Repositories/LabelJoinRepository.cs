using todo_app.core;
using todo_app.EF.Repositories;

namespace todo_app.EF;

public class LabelJoinRepository<T> : GenericRepository<T>, ILabelJoinRepository<T>
    where T : class
{
    private readonly ApplicationDbContext _context;

    public LabelJoinRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public T GetByCompoundKey(int labelId, int itemId)
    {
        return _context.Set<T>().Find(labelId, itemId)!;
    }
}
