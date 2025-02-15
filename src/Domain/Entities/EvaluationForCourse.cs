namespace Itishnik.Domain.Entities;

public class EvaluationForCourse
{
    private int _evaluation;
    private readonly HashSet<EvaluationForSection> _evaluationsForSections = [];

    public EvaluationForCourse(
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

    public IEnumerable<EvaluationForSection> EvaluationsForSections => _evaluationsForSections;

    public void AddEvaluationForSection(EvaluationForSection evaluationForSection) =>
        _evaluationsForSections.Add(evaluationForSection);
    public int Evaluation
    {
        get => _evaluation;
        set
        {
            if (value is < 0 or > 10)
            {
                throw new ArgumentException("Оценка должна быть в диапазоне от 0 до 10 включительно");
            }

            _evaluation = value;
        }
    }
}
