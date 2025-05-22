using System.Reflection;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = Itishnik.Domain.Entities.File;
using Task = Itishnik.Domain.Entities.Task;
using Role = Itishnik.Domain.Constants.Roles;

namespace Itishnik.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly IUser _user;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUser user) : base(options)
    {
        _user = user;
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

        // Считаем, что неавторизованный пользователь может быть только в билд-тайме
        // В рантайме - пользователь авторизованный _user.Id != null
        
        // Нет фильтра для студента, т.к. он требует обращения к GradedCourses и образует бесконечную рекурсию
        // Счистаем, что при работе со студентами ЗАПРЕЩЕНО обращаться к db.Courses напрямую помимо GradedCourses
        builder.Entity<Course>().HasQueryFilter(course =>
            _user.Id == null ||
            _user.Roles.Contains(Role.Administrator) ||
            (_user.Roles.Contains(Role.Teacher) && course.TeacherId == _user.Id) ||
            _user.Roles.Contains(Role.Student)
        );


        builder.Entity<TaskBlock>().HasQueryFilter(tb =>
            _user.Id == null ||
            _user.Roles.Contains(Role.Administrator) ||
            (_user.Roles.Contains(Role.Teacher) && tb.Course.TeacherId == _user.Id) ||
            (_user.Roles.Contains(Role.Student) && tb.IsPublic && this.GradedCourses.Any(gc => gc.StudentId == _user.Id && gc.CourseId == tb.CourseId))
        );

        builder.Entity<Task>().HasQueryFilter(task =>
            _user.Id == null ||
            _user.Roles.Contains(Role.Administrator) ||
            (_user.Roles.Contains(Role.Teacher) && (task.IsPublic || task.TeacherId == _user.Id)) ||
            (_user.Roles.Contains(Role.Student) &&
                task.TaskBlocks.Any(tb =>
                    tb.IsPublic &&
                    this.GradedCourses.Any(gc =>
                        gc.StudentId == _user.Id && 
                        gc.CourseId == tb.CourseId
                    )
                )
            )
        );

        builder.Entity<GradedCourse>().HasQueryFilter(gc =>
            _user.Id == null ||
            _user.Roles.Contains(Role.Administrator) ||
            (_user.Roles.Contains(Role.Teacher) && gc.Course.TeacherId == _user.Id) ||
            (_user.Roles.Contains(Role.Student) && gc.StudentId == _user.Id)
        );
        
        builder.Entity<GradedTaskBlock>().HasQueryFilter(gtb =>
            _user.Id == null ||
            _user.Roles.Contains(Role.Administrator) ||
            (_user.Roles.Contains(Role.Teacher) && gtb.TaskBlock.Course.TeacherId == _user.Id) ||
            (_user.Roles.Contains(Role.Student) && gtb.StudentId == _user.Id && gtb.TaskBlock.IsPublic)
        );

        builder.Entity<Solution>().HasQueryFilter(s =>
            _user.Id == null ||
            _user.Roles.Contains(Role.Administrator) ||
            (_user.Roles.Contains(Role.Teacher) &&
                this.TaskBlocks
                    .Where(tb => tb.Course.TeacherId == _user.Id)
                    .Any(tb => tb.Tasks.Any(t => t.Id == s.TaskId))
            ) ||
            (_user.Roles.Contains(Role.Student) && s.StudentId == _user.Id)
        );
    }
}
