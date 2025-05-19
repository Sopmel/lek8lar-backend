using Microsoft.EntityFrameworkCore;
using Lek8LarBackend.Models;
using Lek8LarBackend.Data.Entities;



namespace Lek8LarBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public ApplicationDbContext() { }

        public DbSet<User> Users { get; set; }

        public DbSet<GameProgressEntity> GameProgress { get; set; } = null!;





    }
}
