namespace Itishnik.Domain.Entities;

public class GradedTaskBlock
{
    private int? _grade;
    private string? _feedback;
    private readonly HashSet<Solution> _solutions = [];
    
    private GradedTaskBlock() {}

    public GradedTaskBlock(Student student, TaskBlock taskBlock, string? feedback = null)
    {
        Feedback = feedback;
        Student = student;
        StudentId = student.Id;
        TaskBlock = taskBlock;
    }

    public Guid Id { get; private init; }

    public Student Student { get; private init; } = null!;
    public Guid StudentId { get; private init; }

    public TaskBlock TaskBlock { get; private init; } = null!;
    public Guid TaskBlockId { get; private init; }

    public IEnumerable<Solution> Solutions => _solutions;

    public void AddSolution(Solution solution) => _solutions.Add(solution);
    
    public DateTime? StartTime { get; private set; }

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

    public void Start()
    {
        if (StartTime is not null)
            throw new InvalidOperationException("Работа уже начата");
        
        StartTime = DateTime.Now;
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
}
