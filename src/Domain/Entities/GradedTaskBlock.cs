namespace Itishnik.Domain.Entities;

public class GradedTaskBlock
{
    private int _grade;
    private string? _feedback;
    private readonly HashSet<Solution> _solutions = [];
    
    private GradedTaskBlock() {}

    public GradedTaskBlock(Student student, TaskBlock taskBlock, string? feedback = null)
    {
        Feedback = feedback;
        Student = student;
        TaskBlock = taskBlock;
    }

    public Guid Id { get; private init; }
    
    public required Student Student { get; init; }
    public required string StudentId { get; init; }
    
    public required TaskBlock TaskBlock { get; init; }
    public Guid TaskBlockId { get; init; }

    public IEnumerable<Solution> Solutions => _solutions;

    public void AddSolution(Solution solution) => _solutions.Add(solution);

    public string? Feedback
    {
        get => _feedback;
        set
        {
            if (_feedback is not null)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Feedback));
            }
            _feedback = value;
        }
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
