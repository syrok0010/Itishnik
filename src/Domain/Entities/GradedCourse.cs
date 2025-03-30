namespace Itishnik.Domain.Entities;

public class GradedCourse
{
    private int _grade;
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

    public void AddGradedTaskBlock(GradedTaskBlock gradedTaskBlock)
    {
        _gradedTaskBlocks.Add(gradedTaskBlock);
    }

    public int Grade
    {
        get => _grade;
        set
        {
            if (value is < 0 or > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(Grade), "Оценка должна быть в диапазоне от 0 до 10 включительно");
            }
            _grade = value;
        }
    }
}
