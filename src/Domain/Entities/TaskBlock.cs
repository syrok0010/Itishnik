namespace Itishnik.Domain.Entities;

public class TaskBlock
{
    private string _name = null!;
    private readonly List<Task> _tasks = [];
    private readonly List<int> _weights = [];
    private readonly HashSet<File> _files = [];
    private bool _isPublic;
    
    private TaskBlock() {}
    
    public TaskBlock(
        string name, 
        Course course,
        string? description = null)
    {
        Name = name;
        Course = course;
        Description = description;
    }
    
    public Guid Id { get; private init; }

    public Course Course { get; private init; } = null!;
    public Guid CourseId { get; private init; }

    public IEnumerable<Task> Tasks => _tasks;
    public IEnumerable<File> Files => _files;
    public IEnumerable<int> Weights => _weights;

    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public TimeSpan? TimeAllowed { get; private set; }

    public void ChangeTimes(DateTime startTime, DateTime endTime, TimeSpan? timeAllowed)
    {
        if (startTime > endTime)
        {
            throw new ArgumentException("Время начала не должно быть позже времени конца");
        }

        if (timeAllowed == TimeSpan.Zero)
        {
            throw new ArgumentException("Время выполнения не должно быть нулевым");
        }

        StartTime = startTime;
        EndTime = endTime;
        TimeAllowed = timeAllowed;
    }

    public void AddTask(Task task, int weight=0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(weight, nameof(weight));
        _tasks.Add(task);
        _weights.Add(weight);
    }
    
    public void RemoveTask(Task task)
    {
        if (IsPublic)
            throw new InvalidOperationException("Нельзя удалить задачу из опубликованного блока");

        var index = _tasks.IndexOf(task);
        _tasks.RemoveAt(index);
        _weights.RemoveAt(index);
    }

    public void SetWeights(ICollection<int> values)
    {
        if (IsPublic)
            throw new InvalidOperationException("Нельзя удалить задачу из опубликованного блока");
        if (values.Count != _weights.Count)
            throw new ArgumentException("Количества весов не совпадают");
        if (values.Any(v => v is < 0 or > 10))
            throw new ArgumentException("Один из весов вне допустимого диапазона");
        
        _weights.Clear();
        _weights.AddRange(values);
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

            if (StartTime is null || EndTime is null || TimeAllowed is null || StartTime < TimeProvider.System.GetLocalNow())
            {
                throw new InvalidOperationException("Некорректно задано время выполнения работы");
            } 
            
            _isPublic = true;
            
            foreach (var gradedCourse in Course.GradedCourses)
                gradedCourse.AddTaskBlock(this);
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
