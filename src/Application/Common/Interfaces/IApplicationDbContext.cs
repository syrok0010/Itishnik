using Itishnik.Domain.Entities;
using Itishnik.Infrastructure.Identity;
using File = Itishnik.Domain.Entities.File;
using Task = Itishnik.Domain.Entities.Task;

namespace Itishnik.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    
    DbSet<Course> Courses { get; }
    
    DbSet<File> Files { get; }
    
    DbSet<GradedCourse> GradedCourses { get; }
    
    DbSet<GradedTaskBlock> GradedTaskBlocks { get; }
    
    DbSet<Solution> Solutions { get; }
    
    DbSet<Student> Students { get; }
    
    DbSet<Tag> Tags { get; }
    
    DbSet<Task> Tasks { get; }
    
    DbSet<TaskBlock> TaskBlocks { get; }
    
    DbSet<Teacher> Teachers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
