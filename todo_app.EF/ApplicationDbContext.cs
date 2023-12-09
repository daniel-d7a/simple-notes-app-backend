using Microsoft.EntityFrameworkCore;
using todo_app.core.Models;

namespace todo_app.EF
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }
        public DbSet<Note> Notes { get; set; }

    }
}
