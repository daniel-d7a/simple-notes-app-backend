using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using todo_app.core;
using todo_app.core.Models;
using todo_app.core.Models.Auth;
using todo_app.core.Models.Data;

namespace todo_app.EF
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddJsonFile(
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            @"..",
                            "todo_app",
                            "appsettings.json"
                        )
                    )
                    .Build();
                string connectionString = configuration.GetConnectionString("DefaultConnection")!;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Include this line to ensure Identity tables are created
            modelBuilder
                .Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId);

            modelBuilder
                .Entity<Todo>()
                .HasOne(t => t.User)
                .WithMany(u => u.Todos)
                .HasForeignKey(t => t.UserId);

            modelBuilder
                .Entity<Todo>()
                .HasMany(t => t.Entries)
                .WithOne(e => e.Todo)
                .HasForeignKey(t => t.TodoId);

            modelBuilder
                .Entity<UserModel>()
                .HasMany(u => u.Labels)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<LabelNote>().HasKey(l => new { l.LabelId, l.NoteId });
            modelBuilder
                .Entity<LabelNote>()
                .HasOne(ln => ln.Note)
                .WithMany(ln => ln.LabelData)
                .HasForeignKey(ln => ln.NoteId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder
                .Entity<LabelNote>()
                .HasOne(ln => ln.Label)
                .WithMany(ln => ln.LabelNotes)
                .HasForeignKey(ln => ln.LabelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabelTodo>().HasKey(l => new { l.LabelId, l.TodoId });
            modelBuilder
                .Entity<LabelTodo>()
                .HasOne(ln => ln.Todo)
                .WithMany(ln => ln.LabelData)
                .HasForeignKey(ln => ln.TodoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<LabelTodo>()
                .HasOne(ln => ln.Label)
                .WithMany(ln => ln.LabelTodos)
                .HasForeignKey(ln => ln.LabelId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(
                    e =>
                        e.Entity is BaseModel
                        && (e.State == EntityState.Added || e.State == EntityState.Modified)
                );

            foreach (var entry in entries)
            {
                var entity = (BaseModel)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<TodoEntry> TodoEntries { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<LabelNote> LabelNotes { get; set; }
        public DbSet<LabelTodo> LabelTodos { get; set; }
    }
}
