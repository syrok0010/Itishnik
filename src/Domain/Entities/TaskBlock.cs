namespace Itishnik.Domain.Entities;

public class TaskBlock
{
    private string _name = null!;
    private readonly List<Task> _tasks = [];
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

    public IEnumerable<Task> Tasks => _tasks;
    public IEnumerable<File> Files => _files;
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan TimeAllowed { get; set; }

    public void AddTask(Task task, int weight=0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(weight, nameof(weight));
        _tasks.Add(task);
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

    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            switch (_isPublic)
            {
                case true when value:
                    throw new InvalidOperationException("Блок задач уже опубликован");
                case true when !value:
                    throw new InvalidOperationException("Невозможно скрыть опубликованный блок задач");
                case false when !value:
                    throw new InvalidOperationException("Блок задач не опубликован");
            }

            if (_tasks.Count == 0)
            {
                throw new InvalidOperationException("Невозможно опубликовать пустой блок задач");
            }
    
            if (_weights.Sum() != 10)
            {
                throw new InvalidOperationException("Сумма весов должна равняться 10");
            }
            _isPublic = true;
        }
    }
    
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
