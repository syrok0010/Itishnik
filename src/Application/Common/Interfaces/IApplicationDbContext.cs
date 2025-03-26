using Itishnik.Domain.Entities;

namespace Itishnik.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }
    
    DbSet<Course> Courses { get; }
    
    DbSet<Teacher> Teachers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
