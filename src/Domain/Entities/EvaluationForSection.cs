namespace Itishnik.Domain.Entities;

public class EvaluationForSection
{
    private int _evaluation;
    private string _feedback = null!;
    private readonly HashSet<Solution> _solutions = [];

    public EvaluationForSection(
        int evaluation,
        string feedback,
        Student student,
        Section section
    )
    {
        Evaluation = evaluation;
        Feedback = feedback;
        Student = student;
        Section = section;
    }

    public Guid Id { get; private init; }
    
    public Student Student { get; private init; }
    public Guid StudentId { get; private init; }
    
    public Section Section { get; private init; }
    public Guid SectionId { get; private init; }

    public IEnumerable<Solution> Solutions => _solutions;

    public void AddSolution(Solution solution) => _solutions.Add(solution);

    public string Feedback
    {
        get => _feedback;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Отзыв не может быть пустым");
            }

            _feedback = value;
        }
    }
    
    public int Evaluation
    {
        get => _evaluation;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Оценка должна быть в диапазоне от 0 до 10 включительно");
            }

            _evaluation = value;
        }
    }
}
