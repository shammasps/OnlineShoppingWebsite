using Microsoft.EntityFrameworkCore;

namespace CrudProject.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User>users{get;set;}
        public DbSet<Product>products{get;set;}
    }
}
