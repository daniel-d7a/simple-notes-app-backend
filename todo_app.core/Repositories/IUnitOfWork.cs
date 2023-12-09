using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_app.core.Models;

namespace todo_app.core.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        IGenericRepository<Note> Notes { get; } 

        Task<int> SaveChanges();

    }
}
