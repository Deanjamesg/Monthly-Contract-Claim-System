using Microsoft.EntityFrameworkCore;
using CMCS_Web_App.Models;

namespace CMCS_Web_App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<UserClaim> UserClaim { get; set; }

    }
}
