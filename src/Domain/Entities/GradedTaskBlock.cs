namespace Itishnik.Domain.Entities;

public class GradedTaskBlock
{
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

        foreach (var entry in TaskBlock.TasksEntries) 
            AddSolution(new Solution(entry.Task, Student, "Здесь будет текст вашего решения"));
 
        StartTime = DateTime.UtcNow;
    }

    public int? Grade
    {
        get
        {
            if (StartTime is null)
                return TaskBlock.EndTime >= DateTime.UtcNow ? null : 0;

            if (_solutions.Count == 0)
                throw new InvalidOperationException("Не загружены решения");

            return Solutions.Where(x => x.Grade != null).Sum(x => x.Grade);
        }
    }
}
