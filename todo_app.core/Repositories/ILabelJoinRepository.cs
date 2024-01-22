using todo_app.core.Repositories;

namespace todo_app.core;

public interface ILabelJoinRepository<T> : IGenericRepository<T>
    where T : class
{
    T GetByCompoundKey(int labelId, int itemId);
}
