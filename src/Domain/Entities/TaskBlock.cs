namespace Itishnik.Domain.Entities;

public class TaskBlock
{
    private string _name = null!;
    private readonly List<Task> _problems = [];
    private readonly List<int> _weights = [];
    private readonly HashSet<File> _files = [];
    private bool _isPublic;
    
    public TaskBlock(string name, Course course)
    {
        Name = name;
        Course = course;
    }
    
    public Guid Id { get; private init; }
    
    public Course Course { get; private init; }
    public Guid CourseId { get; private init; }

    public IEnumerable<Task> Problems => _problems;
    public IEnumerable<File> Files => _files;
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan TimeGate { get; set; }

    public void AddProblem(Task task, int weight=0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(weight, nameof(weight));
        _problems.Add(task);
        _weights.Add(weight);
    }

    public void ChangeWeight(int taskNumber, int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskNumber, nameof(taskNumber));
        if (value is < 0 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Значение веса должно быть от 0 до 10");
        }
        if (_weights.Count < taskNumber)
        {
            throw new ArgumentException($"В блоке всего {_weights.Count} задач");
        }
        
        _weights[taskNumber - 1] = value;
    }

    public void AddFile(File file)
    {
        _files.Add(file);
    }

    private void Publish()
    {
        if (_isPublic)
        {
            return;
        }
    
        if (_problems.Count == 0)
        {
            throw new InvalidOperationException("Невозможно опубликовать пустой блок задач");
        }
    
        if (_weights.Sum() != 10)
        {
            throw new InvalidOperationException("Сумма весов должна равняться 10");
        }
    
        _isPublic = true;
    }

    public bool IsPublic() => _isPublic;
    
    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Name));
            _name = value;
        }
    }

    public string? Description { get; set; }
}
