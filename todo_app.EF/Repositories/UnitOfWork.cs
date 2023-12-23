using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todo_app.core.Models.Data;
using todo_app.core.Repositories;

namespace todo_app.EF.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenericRepository<Note> Notes { get; private set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Notes = new GenericRepository<Note>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
