using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using todo_app.core.Models;
using todo_app.core.Models.Data;
using todo_app.core.Models.Auth;
using Microsoft.Extensions.Configuration;

namespace todo_app.EF
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(){}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
            //optionsBuilder.UseSqlServer(); 
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Include this line to ensure Identity tables are created
            modelBuilder.Entity<Note>()
                        .HasOne(n => n.User)
                        .WithMany(u => u.Notes)
                        .HasForeignKey(n => n.UserId);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken =
            default)
        {
            var entries =
                ChangeTracker.Entries()
                .Where(
                    e => e.Entity is BaseModel
                                 && (e.State == EntityState.Added
                                             || e.State == EntityState.Modified));

            foreach(var entry in entries)
            {
                var entity = (BaseModel)entry.Entity;
                if(entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.UpdatedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Note> Notes { get; set; }
    }
}
