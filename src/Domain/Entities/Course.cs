namespace Itishnik.Domain.Entities;

public class Course
{
    private string _name = null!;
    private readonly HashSet<GradedCourse> _gradedCourses = [];
    private readonly HashSet<TaskBlock> _taskBlocks = [];
    
    public Guid Id { get; private init; }

    public Teacher Teacher { get; private set; } = null!;
    public Guid TeacherId { get; private set; }
    
    private Course() {}

    public Course(Teacher teacher, string name, string? description = null)
    {
        Name = name;
        Description = description;
        Teacher = teacher;
    }
    
    public IEnumerable<TaskBlock> TaskBlocks => _taskBlocks;

    public IEnumerable<GradedCourse> GradedCourses => _gradedCourses;

    public void AddTaskBlock(TaskBlock taskBlock)
    {
        _taskBlocks.Add(taskBlock);
    }
    
    public void DeleteTaskBlock(TaskBlock taskBlock)
    {
        if (taskBlock.IsPublic)
        {
            throw new InvalidOperationException("Опубликованный блок задач не может быть удален");
        }

        _taskBlocks.Remove(taskBlock);
    }

    public void AddGradedCourse(GradedCourse gradedCourse)
    {
        _gradedCourses.Add(gradedCourse);
    }

    public string Name
    {
        get => _name;
        private set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            if (_gradedCourses.Count != 0)
            {
                throw new InvalidOperationException("Невозможно поменять название курса, пока есть студенты");
            }
            _name = value;
        }
    }

    public string? Description { get; set; }
}
