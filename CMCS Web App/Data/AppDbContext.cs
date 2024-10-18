using Microsoft.EntityFrameworkCore;
using CMCS_Web_App.Models;

namespace CMCS_Web_App.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        DbSet<AcademicManager> AcademicManagers { get; set; }
        DbSet<ProgrammeCoordinator> ProgrammeCoordinators { get; set; }
        DbSet<Lecturer> Lecturers { get; set; }

    }
}
