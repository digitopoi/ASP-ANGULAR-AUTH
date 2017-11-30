using ASP_Angular_Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP_Angular_Auth.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
                :base(options) { }

        public DbSet<User> Users { get; set; }
    }
}