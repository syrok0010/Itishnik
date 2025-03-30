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
    
    public required Course Course { get; init; }
    public Guid CourseId { get; init; }
    
    public required Student Student { get; init; }
    public required string StudentId { get; init; }

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
