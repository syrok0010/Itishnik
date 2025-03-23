namespace Itishnik.Domain.Entities;

public class GradedCourse
{
    private int _evaluation;
    private readonly HashSet<GradedTaskBlock> _evaluationsForSections = [];

    public GradedCourse(
        Course course,
        Student student,
        int evaluation
    )
    {
        Course = course;
        Student = student;
        Evaluation = evaluation;
    }
    
    public Guid Id { get; private init; }
    
    public Course Course { get; private init; }
    public Guid CourseId { get; private init; }
    
    public Student Student { get; private init; }
    public Guid StudentId { get; private init; }

    public IEnumerable<GradedTaskBlock> EvaluationsForSections => _evaluationsForSections;

    public void AddEvaluationForSection(GradedTaskBlock gradedTaskBlock)
    {
        _evaluationsForSections.Add(gradedTaskBlock);
    }

    public int Evaluation
    {
        get => _evaluation;
        set
        {
            if (value is < 0 or > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(Evaluation), "Оценка должна быть в диапазоне от 0 до 10 включительно");
            }
            _evaluation = value;
        }
    }
}
