using Microsoft.EntityFrameworkCore;
using Lek8LarBackend.Models;

namespace Lek8LarBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public ApplicationDbContext() { }

        public DbSet<User> Users { get; set; }
    }
}
