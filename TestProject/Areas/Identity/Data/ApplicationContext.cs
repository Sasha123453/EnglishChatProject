using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestProject.Areas.Identity.Data;
using TestProject.Models;

namespace TestProject.Areas.Identity.Data;

public class ApplicationContext : IdentityDbContext<User>
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseSolution> ExerciseSolutions { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
