using Microsoft.EntityFrameworkCore;
using CMCS_Web_App.Models;

namespace CMCS_Web_App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        DbSet<AcademicManager> AcademicManager { get; set; }
        DbSet<ProgrammeCoordinator> ProgrammeCoordinator { get; set; }
        DbSet<Lecturer> Lecturer { get; set; }
        public DbSet<Claim> Claim { get; set; }

    }
}
