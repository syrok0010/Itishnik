namespace Itishnik.Domain.Entities;

public class GradedCourse : IOwnedResource
{
    private int? _grade;
    private readonly HashSet<GradedTaskBlock> _gradedTaskBlocks = [];
    
    private GradedCourse() {}

    public GradedCourse(Course course, Student student)
    {
        Course = course;
        Student = student;
    }
    
    public Guid Id { get; private init; }

    public Course Course { get; private init; } = null!;
    public Guid CourseId { get; private init; }

    public Student Student { get; private init; } = null!;
    public Guid StudentId { get; private init; }

    public IEnumerable<GradedTaskBlock> GradedTaskBlocks => _gradedTaskBlocks;

    public void AddTaskBlock(TaskBlock taskBlock)
    {
        if (!taskBlock.IsPublic)
            throw new InvalidOperationException("Студенту нельзя добавить неопубликованную работу");
        
        _gradedTaskBlocks.Add(new GradedTaskBlock(Student, taskBlock));
    }

    public int? Grade
    {
        get => _grade;
        set => 
            _grade = value switch 
            { 
                null => throw new ArgumentNullException(nameof(value), nameof(Grade)), 
                < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(Grade), "Оценка должна быть в диапазоне от 0 до 10 включительно"), 
                _ => value 
            };
    }

    public Guid GetOwnerId() => StudentId;
}
