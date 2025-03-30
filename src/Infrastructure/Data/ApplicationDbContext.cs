using System.Reflection;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Itishnik.Domain.Entities.File;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<File> Files => Set<File>();
    public DbSet<GradedCourse> GradedCourses => Set<GradedCourse>();
    public DbSet<GradedTaskBlock> GradedTaskBlocks => Set<GradedTaskBlock>();
    public DbSet<Solution> Solutions => Set<Solution>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<TaskBlock> TaskBlocks => Set<TaskBlock>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
