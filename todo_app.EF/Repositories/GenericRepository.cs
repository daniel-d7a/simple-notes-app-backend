using Microsoft.EntityFrameworkCore;
using todo_app.core.Repositories;

namespace todo_app.EF.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        protected ApplicationDbContext _context { get; set; }
        public GenericRepository(ApplicationDbContext context) {
            _context = context;
        }
        public async Task<T> GetOneByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public void Create(T data)
        {
            _context.Set<T>().Add(data);
        }

        public void Update(T data)
        {
            _context.Set<T>().Update(data);
        }

        public void Delete(T data)
        {
            _context.Set<T>().Remove(data);
        }
    }
}
